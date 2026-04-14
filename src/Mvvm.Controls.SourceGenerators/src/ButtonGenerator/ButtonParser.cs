using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Threading;
using System.Linq;

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
        public static DataOrDiagnostics<ButtonAttributeData> GetInfoOrDiagnostic(GeneratorAttributeSyntaxContext nodeContext, CancellationToken token)
        {
            if (nodeContext.TargetSymbol  is not IMethodSymbol method) return default;
            if (Diagnostics.IsNotPartialClass(nodeContext.TargetNode, token, out var diagnostic))
            {
                return new(diagnostic);
            }

            if (
                method.Parameters.Length > 2
                || (method.ReturnsVoid && method.Parameters.Length > 1)
                || (method.ReturnType.Name.Contains("Task") && method.Parameters.Length == 2 && method.Parameters[1].Type.Name != nameof(CancellationToken))
                )
            {
                return new(Diagnostic.Create(Diagnostics.RelayCommandArgumentsInvalid, nodeContext.TargetNode.GetLocation(), method.ToDisplayString()));
            }

            return new (new ButtonAttributeData(nodeContext.TargetNode as MethodDeclarationSyntax, method, nodeContext.SemanticModel, nodeContext.Attributes.First()));
        }
    }
}
