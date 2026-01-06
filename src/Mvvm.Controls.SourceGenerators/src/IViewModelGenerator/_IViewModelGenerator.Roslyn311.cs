using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public class IViewModelGeneratorRoslyn311 : ISourceGenerator
    {
        private class ViewModelContextReceiver : ContextReceiver<GeneratorAttributeSyntaxContext>
        {
            public static ViewModelContextReceiver Create() => new();
            private ViewModelContextReceiver() : base
                (
                IViewModelParser.QualifiedName,
                IViewModelParser.Selector,
                IViewModelParser.GetDataOrDiagnostics
                )
            {
                TransformFieldDeclarations = false; // Select FieldDeclarationSyntax nodes, but transform into VariableDeclarators
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.ForAttributeWithMetaDataName(ViewModelContextReceiver.Create);
            GeneratorExtensions.DebuggerLaunch();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not ViewModelContextReceiver receiver) return;
            if (receiver.AnyCandidates is false) return;

            if (context.Compilation is not CSharpCompilation compilation) return;
            if (compilation.LanguageVersion <= LanguageVersion.CSharp8)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.LanguageVersionTooLow, null, LanguageVersion.CSharp8));
                return;
            }

            var candidates = receiver
                .ReportAndEnumerate(context.ReportDiagnostic, context.CancellationToken);

            foreach (var candidate in candidates)
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var writer = IViewModelEmitter.Emit(candidate.TargetNode, candidate.TargetSymbol, context.ReportDiagnostic, context.CancellationToken);

                if (writer is not null)
                {
                    context.AddSource(writer.SuggestedFileName, writer.ToSourceText());
                }
            }
        }
    }
}
