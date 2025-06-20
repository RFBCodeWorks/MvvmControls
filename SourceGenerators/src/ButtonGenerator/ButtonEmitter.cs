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
            if (data.TargetSymbol is not IMethodSymbol symbol) return;

            bool isAsync = data.TargetSymbol.ReturnType.Name.Contains("Task");
            var parmeterType = string.Empty;
            if (data.TargetSymbol.Parameters.Length >= 1)
            {
                if (!isAsync || isAsync && symbol.Parameters[0].Name != nameof(CancellationToken))
                {
                    parmeterType = $"<{symbol.Parameters[0].Type.ToDisplayString(SymbolFormats.FullyQualifiedFormat)}>";
                }
            }

            // get strings
            if (MvvmDiagnostics.TryGetPropertyName(symbol, "Button", out string fieldName, out string PropName) is Diagnostic nameDiag)
            {
                reportDiagnostic(nameDiag);
                return;
            }

            string buttonType = isAsync ? $"global::RFBCodeWorks.Mvvm.AsyncButtonDefinition{parmeterType}" : $"global::RFBCodeWorks.Mvvm.ButtonDefinition{parmeterType}";

            // get writer
            if (Writer is null)
            {
                Writer ??= new SourceWriter(_token).WriteFileHeader(symbol.ContainingType);
            }
            else
            {
                Writer.WriteLine();
            }

            // write the field
            Writer
                .WriteLine("/// <summary> backing field for <see cref=\"{0}\" /> </summary>", PropName)
                .WriteLine(SourceWriter.GeneratedCodeAttribute)
                .WriteLine(SourceWriter.ExcludeFromCodeCoverage)
                .WriteLine("private {0} {1};", buttonType, fieldName)
                .WriteLine()
                .WriteLine("/// <summary> Generated <see cref=\"{0}\"/> for <see cref=\"{1}\"/> </summary>", buttonType, data.TargetSymbol.Name)
                .WriteLine(SourceWriter.GeneratedCodeAttribute)
                .WriteLine(SourceWriter.ExcludeFromCodeCoverage)
                .WriteIndent().Write("public {0} {1} => {2} ??= new {3}(", buttonType, PropName, fieldName, buttonType);
            
            if (isAsync)
            {
                // create a new command
                Writer.Write("new global::CommunityToolkit.Mvvm.Input.AsyncRelayCommand(").Write(symbol.Name);
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
                Writer.Write(')');
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
            
            // close out the constructor
            Writer.Write(')');

            // write the remainder of the constructor for the button
            if (string.IsNullOrWhiteSpace(data.DisplayName) is false)
            {
                Writer.WriteLine()
                    .WriteLine('{')
                    .WriteLine("    DisplayText = {0}", data.DisplayName)
                    .WriteIndent().Write('}');
            }
            Writer.Write(';').WriteLine();

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
                    .WriteLine("/// <summary>The backing field for <see cref=\"{0}\"/>.</summary>", PropName)
                    .WriteLine(SourceWriter.GeneratedCodeAttribute)
                    .WriteLine("private global::System.Windows.Input.ICommand? {0}CancelCommand;", fieldName)
                    .WriteLine("/// <summary>Gets an <see cref=\"global::System.Windows.Input.ICommand\"/> instance that can be used to cancel <see cref=\"{0}\"/>.</summary>", PropName)
                    .WriteLine(SourceWriter.GeneratedCodeAttribute)
                    .WriteLine(SourceWriter.ExcludeFromCodeCoverage)
                    .WriteLine("public global::System.Windows.Input.ICommand {0}CancelCommand => {1}CancelCommand ??= global::CommunityToolkit.Mvvm.Input.IAsyncRelayCommandExtensions.CreateCancelCommand({0});", PropName, fieldName);
            }
        }
    }
}
