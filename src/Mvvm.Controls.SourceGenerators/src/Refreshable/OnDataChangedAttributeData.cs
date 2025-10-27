using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#nullable enable
#nullable disable warnings

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    /// <summary>
    /// Helper for :
    /// <br/> OnSelectionChangedAttribute
    /// <br/> OnItemSourceChangedAttribute
    /// </summary>
    internal readonly struct OnDataChangedAttributeData : IEquatable<OnDataChangedAttributeData>
    {
        public const string QualifiedName_SelectionChanged = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.OnSelectionChangedAttribute);
        public const string QualifiedName_CollectionChanged = "RFBCodeWorks.Mvvm." + nameof(RFBCodeWorks.Mvvm.OnCollectionChangedAttribute);
        
        // Attribute-specific members:
        public ImmutableArray<string> CommandsToNotify { get; }
        public ImmutableArray<string> MethodsToInvoke { get; }
        public Diagnostic? Diagnostic { get; }

        private OnDataChangedAttributeData(Diagnostic diagnostic, ImmutableArray<string> methods, ImmutableArray<string> commands)
        {
            Diagnostic = diagnostic;
            MethodsToInvoke = methods;
            CommandsToNotify = commands;
        }

        public static OnDataChangedAttributeData GetSelectionChangedData(ISymbol symbol) => GetData(symbol, QualifiedName_SelectionChanged);
        public static OnDataChangedAttributeData GetCollectionChangedData(ISymbol symbol) => GetData(symbol, QualifiedName_CollectionChanged);
        private static OnDataChangedAttributeData GetData(ISymbol symbol, string qualifiedName)
        {
            var attributes = symbol.GetAttributes()
             .Where(a => a.AttributeClass.ToDisplayString() == qualifiedName);

            var methods = ImmutableArray.CreateBuilder<string>();
            var commandbuilder = ImmutableArray.CreateBuilder<string>();

            foreach (var attribute in attributes)
            {
                // Parse constructor arguments
                if (attribute.ConstructorArguments.Length == 1)
                {
                    var arg = attribute.ConstructorArguments[0];
                    if (arg.Kind == TypedConstantKind.Array)
                    {
                        commandbuilder.AddRange(arg.Values.Select(static s => s.Value as string).Where(static s => !string.IsNullOrWhiteSpace(s)));
                    }
                }
                if (attribute.NamedArguments.Length >= 1)
                {
                    var m = attribute.NamedArguments.FirstOrDefault(n => n.Key == "MethodName");
                    if (m.Key != null && m.Value.Value is string s) methods.Add(s);
                }
            }
            return new OnDataChangedAttributeData(null, methods.ToImmutable(), commandbuilder.ToImmutable());
        }

        public bool Equals(OnDataChangedAttributeData other) =>
            Diagnostic == other.Diagnostic
            && CommandsToNotify.SequenceEqual(other.CommandsToNotify)
            && MethodsToInvoke.SequenceEqual(other.MethodsToInvoke);

        public override bool Equals(object obj) => obj is OnDataChangedAttributeData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Diagnostic);
            foreach (var cmd in CommandsToNotify) hash.Add(cmd);
            foreach (var cmd in MethodsToInvoke) hash.Add(cmd);
            return hash.ToHashCode();
        }

        public static bool operator ==(OnDataChangedAttributeData left, OnDataChangedAttributeData right) => left.Equals(right);
        public static bool operator !=(OnDataChangedAttributeData left, OnDataChangedAttributeData right) => !left.Equals(right);

    }
}

