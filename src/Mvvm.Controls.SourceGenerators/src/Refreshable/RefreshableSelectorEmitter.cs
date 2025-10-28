using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator;
using System;
using System.ComponentModel;
using System.Linq;
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
                .WriteLine()
                .WriteLine("/// <summary> Generated <see cref=\"{0}\"/> for <see cref=\"{1}\"/> </summary>", propType.SanitizeForXmlComment(), data.TargetSymbol.Name)
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
            WriteOnCollectionChanged(Writer, dataAndTriggers.CollectionChanged, propName, reportDiagnostic, _token);

            // OnSelectionChanged
            WriteOnSelectionChanged(Writer, dataAndTriggers.SelectionChanged, dataAndTriggers.RefreshData, propName, reportDiagnostic, _token);


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
        private static SourceWriter WriteOnCollectionChanged(SourceWriter writer, in OnDataChangedAttributeData data, ReadOnlySpan<char> propertyName, Action<Diagnostic> reportDiagnostic, CancellationToken token)
        {
            if (data.HasData)
            {
                writer.Write(',').WriteLine()
                    .BeginBlock($"onCollectionChanged: () => ");

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
            return writer;
        }

        /// <inheritdoc cref="WriteOnCollectionChanged(SourceWriter, in OnDataChangedAttributeData, Action{Diagnostic})"/>
        private static SourceWriter WriteOnSelectionChanged(SourceWriter writer, in OnDataChangedAttributeData data, in TriggersRefreshData refreshData, ReadOnlySpan<char> propertyName, Action<Diagnostic> reportDiagnostic, CancellationToken token)
        {
            if (data.HasData || refreshData.Any)
            {
                writer.Write(',').WriteLine()
                    .BeginBlock($"onSelectionChanged: () => ");

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
            return writer;
        }
    }
}
