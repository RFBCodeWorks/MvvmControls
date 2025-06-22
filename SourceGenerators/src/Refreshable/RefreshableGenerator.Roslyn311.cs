using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    // VS2019 only uses 1 generator to generate all Refreshables. This is to reduce load since they share candidate methods. 
    [Generator]
    public class RefreshableSelectorGeneratorRoslyn311 : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RefreshableSelectorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.CancellationToken.IsCancellationRequested || context.SyntaxReceiver is not RefreshableSelectorSyntaxReceiver receiver) return;
            if (receiver.Candidates is null) return;
            if (context.Compilation is not CSharpCompilation compilation) return;


            if (compilation.LanguageVersion <= LanguageVersion.CSharp8)
            {
                context.ReportDiagnostic(Diagnostic.Create(MvvmDiagnostics.LanguageVersionTooLow, null, LanguageVersion.CSharp8));
                return;
            }

            // handles cases where a method may have both ListBoxAttribute and ComboBoxAttribute
            List<DataOrDiagnostics<RefreshableSelectorData>> candidates = null;

            foreach (var node in receiver.Candidates) 
            {
                context.CancellationToken.ThrowIfCancellationRequested();

                if (node is not MethodDeclarationSyntax method
                || compilation.GetSemanticModel(node.SyntaxTree) is not SemanticModel sm
                || sm.GetDeclaredSymbol(method, context.CancellationToken) is not IMethodSymbol symbol
                ) 
                { 
                    continue; 
                }

                foreach (var attr in symbol.GetAttributes())
                {
                    context.CancellationToken.ThrowIfCancellationRequested();

                    string attrDisplayStr = attr.AttributeClass.ToDisplayString(SymbolFormats.NameAndContainingTypes);
                    // ComboBox
                    if (attrDisplayStr.Equals(RefreshableSelectorParser.QualifiedName_ComboBox))
                    {
                        (candidates ??=new()).Add(RefreshableSelectorParser.GetInfoOrDiagnostic(node, sm, symbol, attr, context.CancellationToken));
                    }
                    // ListBox
                    if (attrDisplayStr.Equals(RefreshableSelectorParser.QualifiedName_ListBox))
                    {
                        (candidates ??= new()).Add(RefreshableSelectorParser.GetInfoOrDiagnostic(node, sm, symbol, attr, context.CancellationToken));
                    }
                }
            }

            if (candidates is null || candidates.Count == 0) return;

            var groups = candidates
                .ReportAndEnumerate(context.ReportDiagnostic, context.CancellationToken)
                .GroupBy(d => d.TargetSymbol.ContainingType, SymbolEqualityComparer.Default)
                .Select(d => new GroupedCandidates<RefreshableSelectorData>(d.Key as INamedTypeSymbol, d.ToImmutableArray()));

            foreach (var candidate in groups)
            {
                RefreshableSelectorEmitter emitter = new RefreshableSelectorEmitter(context.CancellationToken);
                
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

        private class RefreshableSelectorSyntaxReceiver : ISyntaxReceiver
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
