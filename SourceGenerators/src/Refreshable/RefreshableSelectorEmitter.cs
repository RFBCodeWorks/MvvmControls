using Microsoft.CodeAnalysis;
using System;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal class RefreshableSelectorEmitter
    {
        private readonly CancellationToken _token;
        
        public SourceWriter Writer { get; private set; }

        public RefreshableSelectorEmitter(CancellationToken token) 
        {
            _token = token;
        }

        internal void EmitProperty(RefreshableSelectorData data, Action<Diagnostic> reportDiagnostic)
        {
            _token.ThrowIfCancellationRequested();

            // get method info
            if (data.TargetSymbol is not IMethodSymbol symbol) return;

            // get strings
            if (MvvmDiagnostics.TryGetPropertyName(symbol, data.PropertySuffix, out string fieldName, out string PropName) is Diagnostic nameDiag)
            {
                reportDiagnostic(nameDiag);
                return;
            }
            string propType = $"{data.TypeToGenerate}{data.CombinedType}";

            // get writer
            if (Writer is null)
            {
                Writer ??= new SourceWriter(_token).WriteFileHeader(data.TargetSymbol.ContainingType);
            }
            else
            {
                Writer.WriteLine();
            }

            // write the field
            Writer
                .WriteLine("/// <summary> backing field for <see cref=\"{0}\" /> </summary>", PropName)
                .WriteLine(SourceWriter.GeneratedCodeAttribute)
                .WriteLine(SourceWriter.ExcludeFromCodeCoverage)
                .WriteLine("private {0} {1};", propType, fieldName)
                .WriteLine()
                .WriteLine("/// <summary> Generated <see cref=\"{0}\"/> for <see cref=\"{1}\"/> </summary>", propType, data.TargetSymbol.Name)
                .WriteLine(SourceWriter.GeneratedCodeAttribute)
                .WriteLine(SourceWriter.ExcludeFromCodeCoverage)
                .WriteLine("public {0} {1} => {2} ??= new {3}", propType, PropName, fieldName, propType)
                .BeginBlock("", '(', true);

            Writer.WriteIndent().Write("refresh: {0}", symbol.Name);
            if (data.CanRefresh.IsNotEmpty())
            {
                Writer.Write(',').WriteLine().WriteIndent().Write("canRefresh: {0}", data.CanRefresh);
            }
            Writer.Write(',').WriteLine().WriteIndent().Write("refreshOnFirstCollectionRequest: {0}", data.RefreshOnInitialize ? "true" : "false");

            Writer.WriteOnCollectionChanged(OnDataChangedAttributeData.GetCollectionChangedData(symbol), reportDiagnostic, _token);

            Writer.WriteOnSelectionChanged(
                OnDataChangedAttributeData.GetSelectionChangedData(symbol),
                TriggersRefreshData.GetAllSelectorTargets(symbol, _token),
                reportDiagnostic, _token);

            // close out the constructor
            Writer.EndBlock(true, true);
        }
    }
}
