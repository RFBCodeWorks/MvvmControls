using Microsoft.CodeAnalysis;
using System;
using System.Threading;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    internal static class RefreshableSelectorEmitterExtensions
    {
        /// <summary>
        /// Invokes the RefreshCommand.Execute method on any RefreshableSelectors specified
        /// </summary>
        public static SourceWriter WriteRefreshTriggers(this SourceWriter writer, in TriggersRefreshData data, CancellationToken token)
        {
            if (data.Any)
            {
                foreach(var item in data.TargetsToRefresh)
                {
                    token.ThrowIfCancellationRequested();
                    writer.WriteLine("{0}.RefreshCommand.Execute(null);", item);
                }
            }
            return writer;
        }

        /// <summary>
        /// Generates an anonymous method if needed.
        /// <br/>The writer begins writing assuming that you are on the next valid location in a constructor. Example : 
        /// <para>
        /// Ctor(Arg1, Arg2
        /// <br/> result : Ctor(Arg1, Arg2, MethodName
        /// </para>
        /// <para>
        /// Ctor(Arg1, Arg2
        /// <br/> result : 
        /// <br/>Ctor(Arg1, Arg2,
        /// <br/>() => {
        /// <br/> NotifyCommands
        /// <br/> }
        /// </para>
        /// </summary>
        /// <param name="writer">The writer to append to.</param>
        /// <returns><see langword="true"/> if data was written, otherwise <see langword="false"/></returns>
        public static SourceWriter WriteOnCollectionChanged(this SourceWriter writer, in OnDataChangedAttributeData data, Action<Diagnostic> reportDiagnostic, CancellationToken token)
        {
            if (data.Diagnostic is not null)
            {
                reportDiagnostic(data.Diagnostic);
                return writer;
            }
            if (data.CommandsToNotify.Length > 0 || data.MethodsToInvoke.Length > 0)
            {
                writer.Write(',').WriteLine()
                    .BeginBlock($"onCollectionChanged: () => ");
                foreach (var cmd in data.CommandsToNotify)
                {
                    token.ThrowIfCancellationRequested();
                    writer.WriteLine("{0}.NotifyCanExecuteChanged();", cmd);
                }
                foreach (var cmd in data.MethodsToInvoke)
                {
                    token.ThrowIfCancellationRequested();
                    writer.WriteLine("{0}();", cmd);
                }
                writer.EndBlock(false);
            }
            return writer;
        }

        /// <inheritdoc cref="WriteOnCollectionChanged(SourceWriter, in OnDataChangedAttributeData, Action{Diagnostic})"/>
        public static SourceWriter WriteOnSelectionChanged(this SourceWriter writer, in OnDataChangedAttributeData data, in TriggersRefreshData refreshData, Action<Diagnostic> reportDiagnostic, CancellationToken token)
        {
            if (data.Diagnostic is not null)
            {
                reportDiagnostic(data.Diagnostic);
                if (refreshData.Any == false) return writer;
            }
            if (data.CommandsToNotify.Length > 0 || data.MethodsToInvoke.Length > 0 || refreshData.Any)
            {
                writer.Write(',').WriteLine()
                    .BeginBlock($"onSelectionChanged: () => ");
                foreach (var cmd in data.CommandsToNotify)
                {
                    token.ThrowIfCancellationRequested();
                    writer.WriteLine("{0}.NotifyCanExecuteChanged();", cmd);
                }
                foreach (var cmd in data.MethodsToInvoke)
                {
                    token.ThrowIfCancellationRequested();
                    writer.WriteLine("{0}();", cmd);
                }
                WriteRefreshTriggers(writer,refreshData, token);
                writer.EndBlock(false);
            }
            return writer;
        }
    }
}
