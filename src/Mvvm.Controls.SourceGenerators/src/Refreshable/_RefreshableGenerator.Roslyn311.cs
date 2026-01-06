using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;
using System;

namespace RFBCodeWorks.Mvvm
{
    // VS2019 only uses 1 generator to generate all Refreshables. This is to reduce load since they share candidate methods. 
    [Generator]
    public class RefreshableSelectorGeneratorRoslyn311 : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RefreshableSelectorSyntaxReceiver());
            //GeneratorExtensions.DebuggerLaunch();
        }

        public void Execute(GeneratorExecutionContext context)
        {
            SyntaxNode lastNode = null;
            try
            {
                if (context.CancellationToken.IsCancellationRequested || context.SyntaxReceiver is not RefreshableSelectorSyntaxReceiver receiver) return;
                if (receiver.Candidates is null) return;
                if (context.Compilation is not CSharpCompilation compilation) return;


                if (compilation.LanguageVersion <= LanguageVersion.CSharp8)
                {
                    context.ReportDiagnostic(Diagnostic.Create(Diagnostics.LanguageVersionTooLow, null, LanguageVersion.CSharp8));
                    return;
                }

                // handles cases where a method may have both ListBoxAttribute and ComboBoxAttribute
                List<DataOrDiagnostics<RefreshableSelectorData>> candidates = null;

                foreach (var node in receiver.Candidates)
                {
                    context.CancellationToken.ThrowIfCancellationRequested();
                    lastNode = node;

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
                        if (RefreshableSelectorParser.QualifiedAttributes.Contains(attrDisplayStr))
                        {
                            (candidates ??= new()).Add(RefreshableSelectorParser.GetInfoOrDiagnostic(node, sm, symbol, attr, context.CancellationToken));
                        }
                    }
                }

                if (candidates is null || candidates.Count == 0) return;

                var groups = candidates
                    .ReportAndEnumerate(context.ReportDiagnostic, context.CancellationToken)
                    .Select(s => RefreshableSelectorParser.TransformRefreshableSelectorData(s, context.CancellationToken))
                    .GroupBy(d => d.SelectorData.TargetSymbol.ContainingType, SymbolEqualityComparer.Default)
                    .Select(d => new GroupedCandidates<RefreshableSelectorDataAndTriggers>(d.Key as INamedTypeSymbol, d.ToImmutableArray()));

                foreach (var candidate in groups)
                {
                    RefreshableSelectorEmitter emitter = new RefreshableSelectorEmitter(context.CancellationToken);

                    foreach (var item in candidate.Values)
                    {
                        context.CancellationToken.ThrowIfCancellationRequested();
                        lastNode = item.RefreshData.TargetNode;
                        emitter.EmitProperty(item, context.ReportDiagnostic);
                    }

                    if (emitter.Writer is not null)
                    {
                        context.AddSource(emitter.Writer.SuggestedFileName, emitter.Writer.ToSourceText());
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (System.Exception e)
            {
                context.ReportDiagnostic(Diagnostics.CreateExceptionDiagnostic(e, lastNode?.GetLocation()));
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
