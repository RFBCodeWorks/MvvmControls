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

        const string FlowExceptions = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommandOptions.FlowExceptionsToTaskScheduler";
        const string AllowConcurrent = "global::CommunityToolkit.Mvvm.Input.AsyncRelayCommandOptions.AllowConcurrentExecutions";
        const string AllowAndFlow = AllowConcurrent + " | " + FlowExceptions;


        internal void EmitProperty(RefreshableSelectorData data, Action<Diagnostic> reportDiagnostic)
        {
            _token.ThrowIfCancellationRequested();

            // get method info
            var symbol = data.TargetSymbol;

            // get strings
            if (!MvvmDiagnostics.TryCleanName(data.TargetSymbol, data.PropertySuffix, out string fieldName, out string PropName, out var nameDiag))
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
                .WriteLine("public {0} {1} => {2} ??= new {3}(", propType, PropName, fieldName, propType);

            Writer.Indentation++;
            Writer.WriteIndent().Write("refresh: {0}", symbol.Name);
            if (data.CanRefresh.IsNotEmpty())
            {
                Writer.Write(',').WriteLine().WriteIndent().Write("canRefresh: {0}", data.CanRefresh);
            }
            Writer.Write(',').WriteLine().WriteIndent().Write("refreshOnFirstCollectionRequest: {0}", data.RefreshOnInitialize ? "true" : "false");

            // get OnItemSourceChanged and OnSelectionChanged handlers
            OnDataChangedAttributeData.GetSelectionChangedData(symbol).GenerateAnonymousMethod(Writer, reportDiagnostic, "onSelectionChanged");
            OnDataChangedAttributeData.GetItemSourceChanged(symbol).GenerateAnonymousMethod(Writer, reportDiagnostic, "onItemSourceChanged");

            // close out the constructor
            Writer.Indentation--;
            Writer.Write(");");
        }
    }
}
