using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using System;
using System.Linq;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public sealed class IViewModelGeneratorRoslyn40 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilationData = context.CompilationProvider.Select(CompilationData.GetData);

            var candidates = context.SyntaxProvider.ForAttributeWithMetadataName
                (
                fullyQualifiedMetadataName: IViewModelParser.QualifiedName,
                predicate: IViewModelParser.Selector,
                transform: IViewModelParser.GetDataOrDiagnostics
                )
                .ReportDiagnostics(context)
                ;

            var candidateCompilation = candidates.Combine(compilationData);

            context.RegisterSourceOutput(candidateCompilation, static (context, candidateProvider) =>
            {
                try
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    var candidate = candidateProvider.Left;
                    var compData = candidateProvider.Right;

                    if (Diagnostics.IsLanguageVersionTooLow(compData.LanguageVersion, Microsoft.CodeAnalysis.CSharp.LanguageVersion.CSharp8, out var diagnostic, () => candidate.TargetSymbol.ContainingType.Locations.FirstOrDefault()))
                    {
                        context.ReportDiagnostic(diagnostic);
                        return;
                    }

                    context.CancellationToken.ThrowIfCancellationRequested();

                    var writer = IViewModelEmitter.Emit(candidate.TargetNode, candidate.TargetSymbol, context.ReportDiagnostic, context.CancellationToken);

                    if (writer is not null)
                    {
                        context.AddSource(writer.SuggestedFileName, writer.ToSourceText());
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    context.ReportDiagnostic(Diagnostics.CreateExceptionDiagnostic(ex, candidateProvider.Left.TargetNode.GetLocation()));
                }
            });
        }
    }
}
