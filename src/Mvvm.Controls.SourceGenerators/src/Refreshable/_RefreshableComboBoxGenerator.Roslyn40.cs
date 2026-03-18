using System.Linq;
using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public sealed class RefreshableComboBoxGeneratorRoslyn40 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilationData = context.CompilationProvider.Select(CompilationData.GetData);

            var candidates = context.SyntaxProvider.ForAttributeWithMetadataName
                (
                fullyQualifiedMetadataName: RefreshableSelectorParser.QualifiedName_ComboBox,
                predicate: RefreshableSelectorParser.NodeSelector,
                transform: RefreshableSelectorParser.GetInfoOrDiagnostic
                )
                .ReportDiagnostics(context)
                .Select(RefreshableSelectorParser.TransformRefreshableSelectorData)
                .GroupBy(static c => c.SelectorData.TargetSymbol.ContainingType, SymbolEqualityComparer.Default) // group by containing class
                .Select(static (c,_) => new GroupedCandidates<RefreshableSelectorDataAndTriggers>(c.Key as INamedTypeSymbol, c.Values))
               ;


            var candidateCompilation = candidates.Combine(compilationData);

            context.RegisterSourceOutput(candidateCompilation, RefreshableSelectorGeneratorRoslyn40.RegisterSourceOutput);
        }
    }
}
