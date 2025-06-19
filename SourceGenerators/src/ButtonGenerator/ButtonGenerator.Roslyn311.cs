using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    [Generator]
    internal class ButtonGeneratorRoslyn311 : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new ButtonSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.CancellationToken.IsCancellationRequested || context.SyntaxReceiver is not ButtonSyntaxReceiver receiver) return;
            if (receiver.Candidates is null) return;
            if (context.Compilation is not CSharpCompilation compilation) return;

            
            if (compilation.LanguageVersion <= LanguageVersion.CSharp8)
            {
                context.ReportDiagnostic(Diagnostic.Create(MvvmDiagnostics.LanguageVersionTooLow, null, LanguageVersion.CSharp8));
                return;
            }

            var candidates = receiver.Candidates
                .Select(node =>
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    var sm = compilation.GetSemanticModel(node.SyntaxTree);
                    if (sm is null) return default;
                    return ButtonParser.GetInfoOrDiagnostic(node, sm, context.CancellationToken);
                })
                .ReportAndEnumerate(context.ReportDiagnostic, context.CancellationToken)
                .GroupBy(d => d.TargetSymbol.ContainingType, SymbolEqualityComparer.Default)
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
                

        private class ButtonSyntaxReceiver : ISyntaxReceiver
        {
            public List<MethodDeclarationSyntax> Candidates { get; private set; }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is MethodDeclarationSyntax method && method.AttributeLists.Count > 0)
                {
                    (Candidates ??= new()).Add(method);
                }
            }
        }
    }
}
