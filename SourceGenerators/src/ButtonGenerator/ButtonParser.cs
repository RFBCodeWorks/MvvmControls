using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators.ButtonGenerator
{
    internal static class ButtonParser
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
        public static bool NodeSelector(SyntaxNode node, CancellationToken token = default)
        {
            return node is MethodDeclarationSyntax method && method.AttributeLists.Count > 0;
        }

        /// <summary>
        /// Evalute the node, and add either to the <see cref="Candidates"/> or the <see cref="Diagnostics"/>
        /// </summary>
        public static DataOrDiagnostics<ButtonAttributeData> GetInfoOrDiagnostic(SyntaxNode node, SemanticModel semanticModel, CancellationToken token)
        {
            if (node is not MethodDeclarationSyntax method || semanticModel.GetDeclaredSymbol(method, token) is not IMethodSymbol symbol )
            {
                return default;
            }

            AttributeData? data = null;
            foreach (var attr in symbol.GetAttributes())
            {
                token.ThrowIfCancellationRequested();
                if (attr.AttributeClass.ToDisplayString(SymbolFormats.NameAndContainingTypes).Equals(ButtonAttributeData.QualifiedName))
                {
                    data = attr;
                    break;
                }
            }
            if (data is null) return default;
            return GetInfoOrDiagnostic(node, semanticModel, symbol, data, token);
        }

        public static DataOrDiagnostics<ButtonAttributeData> GetInfoOrDiagnostic(SyntaxNode node, SemanticModel semanticModel, ISymbol symbol, AttributeData attributeData, CancellationToken token)
        {
            if (symbol is not IMethodSymbol method) return default;
            if (MvvmDiagnostics.IsNotPartialClass(node, token, out var diagnostic))
            {
                return new(diagnostic);
            }

            if (
                method.Parameters.Length > 2
                || (method.ReturnsVoid && method.Parameters.Length > 1)
                || (method.ReturnType.Name.Contains("Task") && method.Parameters.Length == 2 && method.Parameters[1].Type.Name != nameof(CancellationToken))
                )
            {
                return new(Diagnostic.Create(MvvmDiagnostics.RelayCommandArgumentsInvalid, node.GetLocation(), symbol.ToDisplayString()));
            }

            return new (new ButtonAttributeData(node as MethodDeclarationSyntax, symbol as IMethodSymbol, semanticModel, attributeData));
        }
    }
}
