using System.Linq;
using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public sealed class RefreshableListBoxGeneratorRoslyn40 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilationData = context.CompilationProvider.Select(CompilationData.GetData);

            var candidates = context.SyntaxProvider.ForAttributeWithMetadataName
                (
                fullyQualifiedMetadataName: RefreshableSelectorParser.QualifiedName_ListBox,
                predicate: RefreshableSelectorParser.NodeSelector,
                transform: RefreshableSelectorParser.GetInfoOrDiagnostic
                )
                .ReportDiagnostics(context)
                .Select(RefreshableSelectorParser.TransformRefreshableSelectorData)
                .GroupBy(static c => c.SelectorData.TargetSymbol.ContainingType, SymbolEqualityComparer.Default) // group by containing class
                .Select(static (c, _) => new GroupedCandidates<RefreshableSelectorDataAndTriggers>(c.Key as INamedTypeSymbol, c.Values))
               ;


            var candidateCompilation = candidates.Combine(compilationData);

            context.RegisterSourceOutput(candidateCompilation, (context, candidateProvider) =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var candidate = candidateProvider.Left;
                var compData = candidateProvider.Right;

                if (candidate.Values.Length == 0) return;

                if (Diagnostics.IsLanguageVersionTooLow(compData.LanguageVersion, Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8, out var diagnostic, () => candidate.ContainingType.Locations.FirstOrDefault()))
                {
                    context.ReportDiagnostic(diagnostic);
                    return;
                }

                var emitter = new RefreshableSelectorEmitter(context.CancellationToken);
                foreach(var item in candidate.Values)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    emitter.EmitProperty(item, context.ReportDiagnostic);
                }

                if (emitter.Writer is not null)
                {
                    context.AddSource(emitter.Writer.SuggestedFileName, emitter.Writer.ToSourceText());
                }
            });
        }
    }
}
