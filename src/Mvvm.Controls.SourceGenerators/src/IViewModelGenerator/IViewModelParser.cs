using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal class IViewModelParser
    {
        public const string QualifiedName = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.IViewModelAttribute);

        /// <summary> Roslyn 311 selector -> Gather ClassDeclarationSyntax nodes which will then be trasnformed </summary>
        public static bool Selector(SyntaxNode node)
        {
#if DEBUG
            if (node is ClassDeclarationSyntax)
            {
                //GeneratorExtensions.DebuggerBreak();
                return true;
            }
            else 
            {
                return false;
            }
#else
            return node is ClassDeclarationSyntax;
#endif
        }

        /// <summary> Roslyn 4 selector -> Gather VariableDeclaratorSyntax nodes directly </summary>
        public static bool Selector(SyntaxNode node, CancellationToken token) => node is ClassDeclarationSyntax;

        /// <summary>
        /// Generates any diagnostic Data for a <see cref="TriggersRefreshData" > struct places on an [ObservableProperty]
        /// </summary>
        public static DataOrDiagnostics<GeneratorAttributeSyntaxContext> GetDataOrDiagnostics(GeneratorAttributeSyntaxContext context, CancellationToken token)
        {
            if (context.TargetNode is not TypeDeclarationSyntax) return default;
            if (context.TargetSymbol is not ITypeSymbol typeSymbol) return default;
            
            if (context.TargetNode.IsNotPartialClass(token, out var partialDiag))
            {
                return new(partialDiag);
            }

            // type must implement INotifyPropertyChanged if base type is not null
            // if base type is null, this generator can implement INotifyPropertyChanged itself
            if (typeSymbol.BaseType is not null && typeSymbol.DoesNotImplementInterface("System.ComponentModel.INotifyPropertyChanged", token, out var ifaceDiag))
            {
                return new(ifaceDiag);
            }
            
            return new(context);
        }        
    }
}
