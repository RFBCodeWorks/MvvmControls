using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using System.Linq;
using RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    [Generator]
    public sealed class RefreshableListBoxGeneratorRoslyn40 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilationData = context.CompilationProvider.Select(CompilationData.GetData);

            var candidates = context.SyntaxProvider.ForAttributeWithMetadataName
                (
                fullyQualifiedMetadataName: Refreshable.RefreshableSelectorParser.QualifiedName_ListBox,
                predicate: Refreshable.RefreshableSelectorParser.NodeSelector,
                transform: Refreshable.RefreshableSelectorParser.GetInfoOrDiagnostic
                )
                .ReportDiagnostics(context)
                .GroupBy(static c => c.TargetSymbol.ContainingType, SymbolEqualityComparer.Default) // group by containing class
                .Select(static (c,_) => new GroupedCandidates<Refreshable.RefreshableSelectorData>(c.Key as INamedTypeSymbol, c.Values))
               ;


            var candidateCompilation = candidates.Combine(compilationData);

            context.RegisterSourceOutput(candidateCompilation, (context, candidateProvider) =>
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                var candidate = candidateProvider.Left;
                var compData = candidateProvider.Right;

                if (candidate.Values.Length == 0) return;

                if (MvvmDiagnostics.IsLanguageVersionTooLow(compData.LanguageVersion, Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8, out var diagnostic, () => candidate.ContainingType.Locations.FirstOrDefault()))
                {
                    context.ReportDiagnostic(diagnostic);
                    return;
                }

                var emitter = new Refreshable.RefreshableSelectorEmitter(context.CancellationToken);
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
