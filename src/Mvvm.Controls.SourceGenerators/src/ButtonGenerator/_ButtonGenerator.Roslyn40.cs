using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator;
using RFBCodeWorks.Mvvm.SourceGenerators.src;
using System;
using System.Linq;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public sealed class ButtonGeneratorRoslyn40 : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var compilationData = context.CompilationProvider.Select(CompilationData.GetData);

            var candidatesWithDiagnostics = context.SyntaxProvider.ForAttributeWithMetadataName
                (
                fullyQualifiedMetadataName: ButtonAttributeData.QualifiedName,
                predicate: ButtonParser.NodeSelector,
                transform: ButtonParser.GetInfoOrDiagnostic
                );

            context.ReportDiagnostics(candidatesWithDiagnostics.Where(static c=> c.IsErrored).Select((c,_) => c.Diagnostics));

            var candidates = candidatesWithDiagnostics
                .Where(static c => c.IsValid)
                .Select(static (c, _) => c.Data)
                .GroupBy(static c => c.TargetSymbol.ContainingType, SymbolEqualityComparer.Default) // group by containing class
                .Select(static (c,_) => new GroupedCandidates<ButtonAttributeData>(c.Key as INamedTypeSymbol, c.Values))
               ;


            var candidateCompilation = candidates.Combine(compilationData);

            context.RegisterSourceOutput(candidateCompilation, static (context, candidateProvider) =>
            {
                Func<Location> getLocation = null;
                try
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

                    var emitter = new ButtonEmitter(context.CancellationToken);
                    foreach (var item in candidate.Values)
                    {
                        getLocation = item.TargetNode.GetLocation;
                        context.CancellationToken.ThrowIfCancellationRequested();
                        emitter.EmitProperty(item, context.ReportDiagnostic);
                    }

                    if (emitter.Writer is not null)
                    {
                        context.AddSource(emitter.Writer.SuggestedFileName, emitter.Writer.ToSourceText());
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception ex)
                {
                    context.ReportDiagnostic(Diagnostics.CreateExceptionDiagnostic(ex, getLocation?.Invoke()));
                }
            });
        }
    }
}
