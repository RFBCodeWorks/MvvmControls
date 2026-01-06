using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

#nullable enable

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    /// <summary>
    /// Emits source for <see cref="IViewModel"/>
    /// </summary>
    internal static class IViewModelEmitter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="reportDiag"></param>
        public static SourceWriter? Emit(SyntaxNode targetNode, ISymbol targetSymbol, Action<Diagnostic> reportDiag, CancellationToken token)
        {
            if (targetNode is not ClassDeclarationSyntax node) return null;
            if (targetSymbol is not ITypeSymbol symbol) return null;

            token.ThrowIfCancellationRequested();

            SourceWriter writer = new SourceWriter(token);
            writer.Reset();

            GeneratorExtensions.DebuggerBreak();
            
            writer
                .WriteFileHeader()
                .BeginBlock(symbol.ContainingNamespace)
                .WriteSymbolModifiers(symbol, isPartial: true)
                .BeginBlock($"class {node.Identifier.Text} : global::RFBCodeWorks.Mvvm.IViewModel")
                .WriteLine("/// <summary> Reference to the ViewModel that owns this viewmodel. </summary>")
                .WriteAttributes(Attributes.GeneratedCodeAttribute | Attributes.ExcludeFromCodeCoverage)
                // beginning of property
                .BeginBlock("public global::RFBCodeWorks.Mvvm.IViewModel? ParentViewModel")
                // get block
                .WriteLine("get => _parentViewModel;")
                // set block
                .BeginBlock("set")
                .BeginBlock("if (_parentViewModel != value)")
                .WriteLine("global::RFBCodeWorks.Mvvm.IViewModel? oldValue = _parentViewModel;")
                .WriteLine("OnParentViewModelChanging(oldValue, value);")
                .WriteLine("_parentViewModel = value;")
                .WriteLine("OnParentViewModelChanged(oldValue, value);")
                .EndBlock() // if
                .EndBlock() // set
                .EndBlock() // property
                .WriteLine("/// <summary> The generated backing field for <see cref=\"ParentViewModel\"/></summary>")
                .WriteLine("private global::RFBCodeWorks.Mvvm.IViewModel? _parentViewModel;")
                .WriteLine()
                .WriteLine("/// <summary> Partial method called before <see cref=\"ParentViewModel\"/> changes </summary>")
                .WriteLine("partial void OnParentViewModelChanging(global::RFBCodeWorks.Mvvm.IViewModel? oldValue, global::RFBCodeWorks.Mvvm.IViewModel? newValue);")
                .WriteLine()
                .WriteLine("/// <summary> Partial method called after <see cref=\"ParentViewModel\"/> changes </summary>")
                .WriteLine("partial void OnParentViewModelChanged(global::RFBCodeWorks.Mvvm.IViewModel? oldValue, global::RFBCodeWorks.Mvvm.IViewModel? newValue);")
                ;

            writer.SetFileName(symbol);

            return writer;
        }
    }
}
