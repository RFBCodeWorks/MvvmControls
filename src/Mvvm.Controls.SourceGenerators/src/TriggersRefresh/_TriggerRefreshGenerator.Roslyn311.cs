using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public class RefreshTriggerGeneratorRoslyn311 : ISourceGenerator
    {
        private class RefreshContextReceiver : ContextReceiver<TriggersRefreshData>
        {
            public static RefreshContextReceiver Create() => new();
            private RefreshContextReceiver() : base
                (
                TriggersRefreshParser.QualifiedName,
                TriggersRefreshParser.Selector,
                TriggersRefreshParser.GetDataOrDiagnostics
                )
            {
                TransformFieldDeclarations = true; // Select FieldDeclarationSyntax nodes, but transform into VariableDeclarators
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.ForAttributeWithMetaDataName(RefreshContextReceiver.Create);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not RefreshContextReceiver receiver) return;
            if (receiver.AnyCandidates is false) return;

            if (context.Compilation is not CSharpCompilation compilation) return;
            if (compilation.LanguageVersion <= LanguageVersion.CSharp8)
            {
                context.ReportDiagnostic(Diagnostic.Create(Diagnostics.LanguageVersionTooLow, null, LanguageVersion.CSharp8));
                return;
            }

            var candidates = receiver
                .ReportAndEnumerate(context.ReportDiagnostic, context.CancellationToken)
                .GroupBy(d => d.TargetSymbol.ContainingType, SymbolEqualityComparer.Default)
                .Select(d => new GroupedCandidates<TriggersRefreshData>(d.Key as INamedTypeSymbol, d.ToImmutableArray()));

            foreach (var candidate in candidates)
            {
                var emitter = new TriggerRefreshEmitter(context.CancellationToken);
                foreach (var item in candidate.Values)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    emitter.Emit(item, context.ReportDiagnostic);
                }

                if (emitter.Writer is not null)
                {
                    context.AddSource(emitter.Writer.SuggestedFileName, emitter.Writer.ToSourceText());
                }
            }
        }
    }
}
