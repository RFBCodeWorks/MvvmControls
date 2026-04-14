using System.Linq;
using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;
using System;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public sealed class RefreshableSelectorGeneratorRoslyn40 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilationData = context.CompilationProvider.Select(CompilationData.GetData);

            var candidates = context.SyntaxProvider.ForAttributeWithMetadataName
                (
                fullyQualifiedMetadataName: RefreshableSelectorParser.QualifiedName_Selector,
                predicate: RefreshableSelectorParser.NodeSelector,
                transform: RefreshableSelectorParser.GetInfoOrDiagnostic
                )
                .ReportDiagnostics(context)
                .Select(RefreshableSelectorParser.TransformRefreshableSelectorData)
                .GroupBy(static c => c.SelectorData.TargetSymbol.ContainingType, SymbolEqualityComparer.Default) // group by containing class
                .Select(static (c, _) => new GroupedCandidates<RefreshableSelectorDataAndTriggers>(c.Key as INamedTypeSymbol, c.Values))
               ;


            var candidateCompilation = candidates.Combine(compilationData);

            context.RegisterSourceOutput(candidateCompilation, RegisterSourceOutput);
        }


        internal static void RegisterSourceOutput(SourceProductionContext context, (GroupedCandidates<RefreshableSelectorDataAndTriggers> candidates, CompilationData compData) data)
        {
            if (context.CancellationToken.IsCancellationRequested || data.candidates.Values.Length == 0) 
                return;
            
            var compData = data.compData;
            var candidates = data.candidates;

            Func<Location> getLocation = null;
            try
            {

                if (Diagnostics.IsLanguageVersionTooLow(compData.LanguageVersion, Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8, out var diagnostic, () => candidates.ContainingType.Locations.FirstOrDefault()))
                {
                    context.ReportDiagnostic(diagnostic);
                    return;
                }

                var emitter = new RefreshableSelectorEmitter(context.CancellationToken);
                foreach (var item in candidates.Values)
                {
                    getLocation = item.SelectorData.TargetNode.GetLocation;
                    context.CancellationToken.ThrowIfCancellationRequested();
                    emitter.EmitProperty(item, context.ReportDiagnostic);
                }

                if (emitter.Writer is not null)
                {
                    context.AddSource(emitter.Writer.SuggestedFileName, emitter.Writer.ToSourceText());
                }
            }catch(OperationCanceledException){ }
            catch(Exception e)
            {
                context.ReportDiagnostic(Diagnostics.CreateExceptionDiagnostic(e, getLocation?.Invoke()));
            }
        }
    }
}
