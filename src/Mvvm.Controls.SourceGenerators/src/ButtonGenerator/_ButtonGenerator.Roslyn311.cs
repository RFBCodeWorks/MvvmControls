using System.Linq;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator;

namespace RFBCodeWorks.Mvvm
{
    [Generator]
    public class ButtonGeneratorRoslyn311 : ISourceGenerator
    {
        private class ButtonSyntaxReceiver : ContextReceiver<ButtonAttributeData>
        {
            public static ButtonSyntaxReceiver Create() => new();
            private ButtonSyntaxReceiver() : base(
                ButtonAttributeData.QualifiedName,
                n => n is MethodDeclarationSyntax,
                ButtonParser.GetInfoOrDiagnostic
                )
            { }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.ForAttributeWithMetaDataName(ButtonSyntaxReceiver.Create);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.CancellationToken.IsCancellationRequested) return;
            if (context.SyntaxContextReceiver is not ButtonSyntaxReceiver receiver) return;
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
                .Where( grouping => grouping.Key is INamedTypeSymbol)
                .Select(d => new GroupedCandidates<ButtonAttributeData>(d.Key as INamedTypeSymbol, d.ToImmutableArray()));

            foreach (var candidate in candidates)
            {
                ButtonEmitter emitter = new ButtonEmitter(context.CancellationToken);
                foreach (var item in candidate.Values)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    emitter.EmitProperty(item, context.ReportDiagnostic);
                }

                if (emitter.Writer is not null)
                {
                    context.AddSource(emitter.Writer.SuggestedFileName, emitter.Writer.ToSourceText());
                }
            }
        }
    }
}
