using Microsoft.CodeAnalysis;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal class RefreshableSelectorEmitter
    {
        private readonly CancellationToken _token;
        
        public SourceWriter Writer { get; private set; }

        public RefreshableSelectorEmitter(CancellationToken token)
        {
            _token = token;
        }


        internal void EmitProperty(RefreshableSelectorDataAndTriggers dataAndTriggers, Action<Diagnostic> reportDiagnostic)
        {
            _token.ThrowIfCancellationRequested();

            var data = dataAndTriggers.SelectorData;

            // get method info
            if (data.TargetSymbol is not IMethodSymbol refreshMethodSymbol) return;

            // get strings
            ReadOnlySpan<char> fieldName, propName;
            if (data.PropertyName.IsNotEmptyOrWhiteSpace())
            {
                propName = data.PropertyName.AsSpan();
                Span<char> fc = new char[propName.Length + 1];
                fc[0] = '_';
                fc[1] = Char.ToLowerInvariant(propName[0]);
                propName.Slice(1).CopyTo(fc.Slice(2));
                fieldName = fc;
            }
            else if (Diagnostics.TryGetPropertyName(refreshMethodSymbol, data.PropertySuffix, out fieldName, out propName) is Diagnostic nameDiag)
            {
                reportDiagnostic(nameDiag);
                return;
            }

            string propType = $"{data.TypeToGenerate}{data.CombinedType}";

            // get writer
            if (Writer is null)
            {
                Writer ??= new SourceWriter(_token).WriteFileHeader(data.TargetSymbol.ContainingType);
            }
            else
            {
                Writer.WriteLine();
            }

            // write the field
            Writer
                .WriteLine("/// <summary> backing field for <see cref=\"{0}\" /> </summary>".AsSpan(), propName)
                .WriteLine(SourceWriter.GeneratedCodeAttribute)
                .WriteLine("private {0}? {1};".AsSpan(), propType.AsSpan(), fieldName)
                .WriteLine();

            /* write the property comment
             * params
             * 0 = fully qualified type of ISelector
             * 1 = Sanitized type name to use as the link text
             * 2 = method name the attribute was applied to
             */
            const string summaryFormatString = "/// Generated <see cref=\"{0}{{T, TList,  TSelectedValue}}\">{1}</see> for <see cref=\"{2}\"/>";
            
            Writer.WriteLine("/// <summary>").WriteLine(summaryFormatString, data.TypeToGenerate, propType.SanitizeForXmlComment("global::RFBCodeWorks.Mvvm"), data.TargetSymbol.Name);
            if (data.TargetSymbol.GetDocumentationCommentXml(null, false, _token) is string comment && comment.Contains("<summary>"))
            {
                Writer.WriteLine("/// <para/><inheritdoc cref=\"{0}\" path=\"/summary\" />", data.TargetSymbol.Name);
            }
            Writer.WriteLine("/// </summary>").WriteLine("/// <inheritdoc cref=\"{0}\" />", data.TargetSymbol.Name);

            // write the property
            Writer
                .WriteAttributes(Attributes.GeneratedCodeAttribute | Attributes.ExcludeFromCodeCoverage)
                .WriteLine("public {0} {1} => {2} ??= new {0}".AsSpan(), propType.AsSpan(), propName, fieldName)
                .BeginBlock("", '(', true);

            // RefreshMethod
            Writer.WriteIndent();
            if (data.IsAsync)
            {
                Writer.Write("{0}: {1}", data.IsCancellable ? "refreshAsyncCancellable" : "refreshAsync", refreshMethodSymbol.Name);
            }
            else
            {
                Writer.Write("refresh: {0}", refreshMethodSymbol.Name);
            }

            // canRefresh
            if (data.CanRefresh.IsNotEmpty())
            {
                Writer.Write(',').WriteLine().WriteIndent().Write("canRefresh: {0}", data.CanRefresh);
            }

            // refreshOnFirstCollectionRequest
            Writer.Write(',').WriteLine().WriteIndent().Write("refreshOnFirstCollectionRequest: {0}", data.RefreshOnInitialize ? "true" : "false");

            // OnCollectionChanged
            Debug.Assert(Writer.Indentation == 3);
            Debug.WriteLine($"DEBUG : fileName [{Path.GetFileName(Writer.SuggestedFileName)}] : Indentation before writing OnCollectionChanged: {Writer.Indentation}");
            
            WriteOnCollectionChanged(Writer, dataAndTriggers.CollectionChanged, propName, _token);

            Debug.WriteLine($"DEBUG : fileName [{Path.GetFileName(Writer.SuggestedFileName)}] : Indentation after writing OnCollectionChanged: {Writer.Indentation}");            
            Debug.Assert(Writer.Indentation == 3);

            // OnSelectionChanged
            WriteOnSelectionChanged(Writer, dataAndTriggers.SelectionChanged, dataAndTriggers.RefreshData, propName, _token);
            
            Debug.WriteLine($"DEBUG : fileName [{Path.GetFileName(Writer.SuggestedFileName)}] : Indentation after writing OnSelectionChanged: {Writer.Indentation}");
            Debug.Assert(Writer.Indentation == 3);

            // write the remainder of the constructor for the button
            bool tt = !string.IsNullOrWhiteSpace(data.ToolTip);
            bool dp = !string.IsNullOrWhiteSpace(data.DisplayMemberPath);
            bool sv = !string.IsNullOrWhiteSpace(data.SelectedValuePath);
            if (tt || dp || sv)
            {
                Writer
                    .EndBlock(true, false)  // close out the constructor ')'
                    .BeginBlock()           // Open properties scope {
                    .WriteLineIf(tt, $@"{nameof(SelectorAttribute.ToolTip)}  = {{0}}", data.ToolTip, trailingComma: dp || sv)
                    .WriteLineIf(dp, $@"{nameof(SelectorAttribute.DisplayMemberPath)}  = {{0}}", data.DisplayMemberPath, trailingComma: sv)
                    .WriteLineIf(sv, $@"{nameof(SelectorAttribute.SelectedValuePath)}  = {{0}}", data.SelectedValuePath)
                    ;
            }
            Writer.EndBlock(true, true);

            WritePartialMethods(Writer, propName, dataAndTriggers.CollectionChanged.HasData, dataAndTriggers.SelectionChanged.HasData);

            Debug.Assert(Writer.Indentation == 2);
        }

        /// <summary>
        /// Generates an anonymous method if needed.
        /// <br/>The writer begins writing assuming that you are on the next valid location in a constructor. Example : 
        /// <para>
        /// Ctor(Arg1, Arg2
        /// <br/> result : Ctor(Arg1, Arg2, Action
        /// </para>
        /// <para>
        /// Ctor(Arg1, Arg2
        /// <br/> result : 
        /// <br/>Ctor(Arg1, Arg2,
        /// <br/>() => {
        /// <br/> NotifyCommands
        /// <br/> }
        /// </para>
        /// </summary>
        /// <param name="writer">The writer to append to.</param>
        /// <returns><see langword="true"/> if data was written, otherwise <see langword="false"/></returns>
        private static void WriteOnCollectionChanged(SourceWriter writer, in OnDataChangedAttributeData data, ReadOnlySpan<char> propertyName,  CancellationToken token)
        {
            if (data.HasData)
            {
                writer.Write(',').WriteLine()
                    .BeginBlock($"onCollectionChanged: () => ");

                writer.WriteLine("On{0}CollectionChanged();", propertyName);

                if (!data.CommandsToNotify.IsDefaultOrEmpty)
                {
                    foreach (var cmd in data.CommandsToNotify)
                    {
                        token.ThrowIfCancellationRequested();
                        writer.WriteLine("{0}?.NotifyCanExecuteChanged();", cmd.AsSpan());
                    }
                }
                if (!data.SelectorActionsToInvoke.IsDefaultOrEmpty)
                {
                    foreach (var cmd in data.SelectorActionsToInvoke)
                    {
                        token.ThrowIfCancellationRequested();
                        writer.WriteLine("{0}({1});", cmd.AsSpan(), propertyName);
                    }
                }
                if (!data.ActionsToInvoke.IsDefaultOrEmpty)
                {
                    foreach (var cmd in data.ActionsToInvoke)
                    {
                        token.ThrowIfCancellationRequested();
                        writer.WriteLine("{0}();", cmd.AsSpan());
                    }
                }
                writer.EndBlock(false);
            }
        }

        /// <inheritdoc cref="WriteOnCollectionChanged(SourceWriter, in OnDataChangedAttributeData, Action{Diagnostic})"/>
        private static void WriteOnSelectionChanged(SourceWriter writer, in OnDataChangedAttributeData data, in TriggersRefreshData refreshData, ReadOnlySpan<char> propertyName, CancellationToken token)
        {
            if (data.HasData || refreshData.Any)
            {
                writer.Write(',').WriteLine()
                    .BeginBlock($"onSelectionChanged: () => ");

                if (data.HasData)
                {
                    writer.WriteLine("On{0}SelectionChanged();", propertyName);
                }

                if (!data.SelectorActionsToInvoke.IsDefaultOrEmpty)
                {
                    foreach (var cmd in data.SelectorActionsToInvoke)
                    {
                        token.ThrowIfCancellationRequested();
                        writer.WriteLine("{0}({1});", cmd.AsSpan(), propertyName);
                    }
                }

                if (!data.ActionsToInvoke.IsDefaultOrEmpty)
                {
                    foreach (var cmd in data.ActionsToInvoke)
                    {
                        token.ThrowIfCancellationRequested();
                        writer.WriteLine("{0}();", cmd);
                    }
                }

                TriggerRefreshEmitter.WriteRefreshTriggers(writer, refreshData, token);

                if (!data.CommandsToNotify.IsDefaultOrEmpty)
                {
                    foreach (var cmd in data.CommandsToNotify)
                    {
                        token.ThrowIfCancellationRequested();
                        writer.WriteLine("{0}?.NotifyCanExecuteChanged();", cmd.AsSpan());
                    }
                }
                
                writer.EndBlock(false);
            }
        }

        private static void WritePartialMethods(SourceWriter writer, ReadOnlySpan<char> propertyName, bool onCollection, bool onSelection)
        {
            if (onCollection)
                writer
                    .WriteLine()
                    .WriteLine("/// <summary> Called when the collection of <see cref=\"{0}\"/> changes. </summary>", propertyName)
                    .WriteAttributes(Attributes.GeneratedCodeAttribute | Attributes.ExcludeFromCodeCoverage)
                    .WriteLine("partial void On{0}CollectionChanged();", propertyName);

            if (onSelection)
                writer
                    .WriteLine()
                    .WriteLine("/// <summary> Called when the selection of <see cref=\"{0}\"/> changes. </summary>", propertyName)
                    .WriteAttributes(Attributes.GeneratedCodeAttribute | Attributes.ExcludeFromCodeCoverage)
                    .WriteLine("partial void On{0}SelectionChanged();", propertyName);
        }
    }
}
