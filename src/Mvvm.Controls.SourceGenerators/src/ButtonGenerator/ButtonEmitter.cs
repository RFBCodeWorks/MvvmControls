using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator
{
    internal class ButtonEmitter
    {
        private readonly CancellationToken _token;
        
        public SourceWriter Writer { get; private set; }

        public ButtonEmitter(CancellationToken token) 
        {
            _token = token;
        }

        const string FlowExceptions = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler";
        const string AllowConcurrent = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommandOptions.AllowConcurrentExecutions";
        const string AllowAndFlow = AllowConcurrent + " | " + FlowExceptions;


        internal void EmitProperty(ButtonAttributeData data, Action<Diagnostic> reportDiagnostic)
        {
            _token.ThrowIfCancellationRequested();

            // get method info
            if (data.TargetSymbol is not IMethodSymbol methodSymbol) return;

            // get strings
            ReadOnlySpan<char> fieldName, propName;
            if (data.PropertyName.IsNotEmptyOrWhiteSpace())
            {
                propName = data.PropertyName.Trim().AsSpan();
                Span<char> fc = new char[propName.Length + 1];
                fc[0] = '_';
                fc[1] = Char.ToLowerInvariant(propName[0]);
                propName.Slice(1).CopyTo(fc.Slice(2));
                fieldName = fc;
            }
            else if (Diagnostics.TryGetPropertyName(methodSymbol, "Button", out fieldName, out propName) is Diagnostic nameDiag)
            {
                reportDiagnostic(nameDiag);
                return;
            }

            bool isAsync = data.TargetSymbol.ReturnType.Name.Contains("Task");
            var parmeterType = string.Empty;
            if (data.TargetSymbol.Parameters.Length >= 1)
            {
                if (!isAsync || isAsync && methodSymbol.Parameters[0].Type.Name != nameof(CancellationToken))
                {
                    parmeterType = $"<{methodSymbol.Parameters[0].Type.ToDisplayString(SymbolFormats.FullyQualifiedFormat)}>";
                }
            }

            string buttonType = isAsync ? $"global::RFBCodeWorks.Mvvm.AsyncButtonDefinition{parmeterType}" : $"global::RFBCodeWorks.Mvvm.ButtonDefinition{parmeterType}";

            // get writer
            if (Writer is null)
            {
                Writer ??= new SourceWriter(_token).WriteFileHeader(methodSymbol.ContainingType);
            }
            else
            {
                Writer.WriteLine();
            }

            // write the field
            Writer
                .WriteLine("/// <summary> backing field for <see cref=\"{0}\" /> </summary>".AsSpan(), propName)
                .WriteLine(SourceWriter.GeneratedCodeAttribute)
                .WriteLine("private {0}? {1};".AsSpan(), buttonType.AsSpan(), fieldName)
                .WriteLine();

            // write the property comment
            if (data.TargetSymbol.GetDocumentationCommentXml(null, false, _token) is string comment && comment.Contains("<summary>"))
            {
                Writer.WriteLine(comment);
            } else
            {
                Writer.WriteLine("/// <summary> Generated <see cref=\"{0}\"/> for <see cref=\"{1}\"/> </summary>", buttonType.SanitizeForXmlComment(), data.TargetSymbol.Name);
            }
            
            // write the property
            Writer
                .WriteAttributes(Attributes.GeneratedCodeAttribute | Attributes.ExcludeFromCodeCoverage)
                .WriteIndent().Write("public {0} {1} => {2} ??= new {0}".AsSpan(), buttonType.AsSpan(), propName, fieldName)
                .BeginBlock("", '(', false);
            
            if (isAsync)
            {
                const string CommunityToolkitAsyncCommand = "new global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand{0}(";
                

                // create a new command
                Writer.Write(CommunityToolkitAsyncCommand, parmeterType).Write(methodSymbol.Name);
                if (string.IsNullOrWhiteSpace(data.CanExecute) is false)
                {
                    Writer.Write(',').Write(' ').Write(data.CanExecute);
                }
                if (data.AllowConcurrentExecutions && data.FlowExceptionsToTaskScheduler)
                    Writer.Write(',').Write(' ').Write(AllowAndFlow);
                else if (data.AllowConcurrentExecutions)
                    Writer.Write(',').Write(' ').Write(AllowConcurrent);
                else if (data.FlowExceptionsToTaskScheduler)
                    Writer.Write(',').Write(' ').Write(FlowExceptions);
                Writer.Write(')'); // end of command ctor
            }
            else
            {
                // use the method directly
                Writer.Write(data.TargetSymbol.Name);
                if (string.IsNullOrWhiteSpace(data.CanExecute) is false)
                {
                    Writer.Write(',').Write(' ').Write(data.CanExecute);
                }
            }
            
            // write the remainder of the constructor for the button
            bool dp = !string.IsNullOrWhiteSpace(data.DisplayText), tt = !string.IsNullOrWhiteSpace(data.ToolTip);
            if (dp || tt)
            {
                Writer
                    .EndBlock()     // end the ctor paramater block
                    .BeginBlock()   // begin the button properties block
                    .WriteLineIf(dp, $@"{nameof(ButtonAttributeData.DisplayText)} = {{0}}", data.DisplayText, trailingComma: tt)
                    .WriteLineIf(tt, $@"{nameof(ButtonAttributeData.ToolTip)}  = {{0}}", data.ToolTip)
                    ;
            }
            Writer.EndBlock(false, true).WriteLine(); // close out the constructor

            if (isAsync && data.IncludeCancelCommand)
            {
                /*
        /// <summary>The backing field for <see cref="TestCancelCommand"/>.</summary>
        [global::System.CodeDom.Compiler.GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
        private global::System.Windows.Input.ICommand? testCancelCommand;
        /// <summary>Gets an <see cref="global::System.Windows.Input.ICommand"/> instance that can be used to cancel <see cref="TestCommand"/>.</summary>
        [global::System.CodeDom.Compiler.GeneratedCode("CommunityToolkit.Mvvm.SourceGenerators.RelayCommandGenerator", "8.4.0.0")]
        [global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        public global::System.Windows.Input.ICommand TestCancelCommand => testCancelCommand ??= global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommandExtensions.CreateCancelCommand(TestCommand);
                 */

                Writer
                    .WriteLine("/// <summary>The backing field for <see cref=\"{0}\"/>.</summary>".AsSpan(), propName)
                    .WriteLine(SourceWriter.GeneratedCodeAttribute)
                    .WriteLine("private global::System.Windows.Input.ICommand? {0}CancelCommand;".AsSpan(), fieldName)
                    .WriteLine("/// <summary>Gets an <see cref=\"global::System.Windows.Input.ICommand\"/> instance that can be used to cancel <see cref=\"{0}\"/>.</summary>".AsSpan(), propName)
                    .WriteLine(SourceWriter.GeneratedCodeAttribute)
                    .WriteLine(SourceWriter.ExcludeFromCodeCoverage)
                    .WriteLine("public global::System.Windows.Input.ICommand {0}CancelCommand => {1}CancelCommand ??= global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommandExtensions.CreateCancelCommand({0});".AsSpan(), propName, fieldName);
            }
        }
    }
}
