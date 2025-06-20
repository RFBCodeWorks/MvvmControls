using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RFBCodeWorks.Mvvm.SourceGenerators.Refreshable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.src.Refreshable
{
    /// <summary>
    /// Emits On(PropName)Changed(returnType value) methods
    /// </summary>
    internal class RefreshTriggerEmitter
    {
        private readonly CancellationToken _token;

        public SourceWriter Writer { get; private set; }

        public RefreshTriggerEmitter(CancellationToken token)
        {
            _token = token;
        }

        public void Emit(TriggersRefreshData data, Action<Diagnostic> reportDiag)
        {
            if (data.TargetNode is not FieldDeclarationSyntax fieldDeclaration) return;
            if (data.Any is false) return;

            string returnType = null;
            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                _token.ThrowIfCancellationRequested();

                if (data.SemanticModel.GetSymbolInfo(variable, _token).Symbol is not IFieldSymbol fieldSymbol) continue;

                if (MvvmDiagnostics.TryGetPropertyName(fieldSymbol, "", out string propName) is Diagnostic d)
                {
                    reportDiag(d);
                    continue;
                }
                returnType ??= fieldSymbol.Type.ToDisplayString(SymbolFormats.FullyQualifiedFormat);

                if (Writer is null)
                {
                    Writer = new SourceWriter(_token).WriteFileHeader(fieldSymbol.ContainingType);
                }
                else
                {
                    Writer.WriteLine();
                }

                Writer
                    .BeginBlock($"partial void On{propName}Changed({returnType} value)")
                    .WriteRefreshTriggers(data, _token)
                    .EndBlock();
            }
        }
    }
}
