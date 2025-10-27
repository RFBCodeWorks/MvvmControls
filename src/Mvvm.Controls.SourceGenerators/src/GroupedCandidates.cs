using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace RFBCodeWorks.Mvvm.SourceGenerators
{
    internal readonly struct GroupedCandidates<T>
    {
        public readonly INamedTypeSymbol ContainingType;
        public readonly ImmutableArray<T> Values;

        public GroupedCandidates(INamedTypeSymbol containingType, ImmutableArray<T> values)
        {
            ContainingType = containingType ?? throw new ArgumentNullException(nameof(containingType));
            Values = values;
        }
    }
}
