using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal readonly struct DataOrDiagnostics<T> where T: struct
    {
        private readonly T? _data;
        public readonly ImmutableArray<Diagnostic> Diagnostics;

        public bool IsErrored => !Diagnostics.IsDefaultOrEmpty;
        public bool IsValid => _data.HasValue;
        public T Data => _data.Value;

        public DataOrDiagnostics(T data)
        {
            _data = data;
            Diagnostics = ImmutableArray<Diagnostic>.Empty;
        }

        public DataOrDiagnostics(ImmutableArray<Diagnostic> diagnostics)
        {
            _data = null;
            Diagnostics = diagnostics;
        }

        public DataOrDiagnostics(Diagnostic diagnostic)
        {
            _data = null;
            Diagnostics = ImmutableArray.Create(diagnostic);
        }
    }

    internal static class DataOrDiagnosticExtensions
    {
        public static IEnumerable<T> ReportAndEnumerate<T>(this IEnumerable<DataOrDiagnostics<T>> collection, Action<Diagnostic> report, CancellationToken token)
            where T : struct
        {
            foreach (var item in collection)
            {
                token.ThrowIfCancellationRequested();
                if (item.IsErrored)
                {
                    foreach (var diagnostic in item.Diagnostics)
                    {
                        token.ThrowIfCancellationRequested();
                        report(diagnostic);
                    }
                }
                else if (item.IsValid)
                {
                    yield return item.Data;
                }
            }
        }
    }
}
