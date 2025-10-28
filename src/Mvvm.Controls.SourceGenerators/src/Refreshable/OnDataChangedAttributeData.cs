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
        public ImmutableArray<string> ActionsToInvoke { get; }
        public ImmutableArray<string> SelectorActionsToInvoke { get; }
        public Diagnostic? Diagnostic { get; }

        /// <summary>
        /// True if any of the commands/actions have data. False if all are default or empty.
        /// </summary>
        public bool HasData =>
            !SelectorActionsToInvoke.IsDefaultOrEmpty ||
            !CommandsToNotify.IsDefaultOrEmpty ||
            !ActionsToInvoke.IsDefaultOrEmpty;

        private OnDataChangedAttributeData(Diagnostic diagnostic, ImmutableArray<string> actions, ImmutableArray<string> commands, ImmutableArray<string> selectorActions)
        {
            Diagnostic = diagnostic;
            ActionsToInvoke = actions;
            CommandsToNotify = commands;
            SelectorActionsToInvoke = selectorActions;
        }

        public static OnDataChangedAttributeData GetSelectionChangedData(ISymbol symbol) => GetData(symbol, QualifiedName_SelectionChanged);
        public static OnDataChangedAttributeData GetCollectionChangedData(ISymbol symbol) => GetData(symbol, QualifiedName_CollectionChanged);
        private static OnDataChangedAttributeData GetData(ISymbol symbol, string qualifiedName)
        {
            var attributes = symbol.GetAttributes()
             .Where(a => a.AttributeClass.ToDisplayString() == qualifiedName);

            var actions = ImmutableArray.CreateBuilder<string>();
            var selectorActions = ImmutableArray.CreateBuilder<string>();
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
                    foreach (var argument in attribute.NamedArguments)
                    {
                        if (argument.Key == nameof(OnCollectionChangedAttribute.Action))
                        {
                            if (argument.Value.Value is string m) actions.Add(m);
                        }
                        else if (argument.Key == nameof(OnCollectionChangedAttribute.SelectorAction))
                        {
                            if (argument.Value.Value is string m) selectorActions.Add(m);
                        }
                    }
                }
            }
            return new OnDataChangedAttributeData(null, actions.ToImmutable(), commandbuilder.ToImmutable(), selectorActions.ToImmutable());
        }

        public bool Equals(OnDataChangedAttributeData other) =>
            Diagnostic == other.Diagnostic
            && CommandsToNotify.SequenceEqual(other.CommandsToNotify)
            && ActionsToInvoke.SequenceEqual(other.ActionsToInvoke);

        public override bool Equals(object obj) => obj is OnDataChangedAttributeData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(this.Diagnostic);
            foreach (var cmd in CommandsToNotify) hash.Add(cmd);
            foreach (var cmd in ActionsToInvoke) hash.Add(cmd);
            foreach (var cmd in SelectorActionsToInvoke) hash.Add(cmd);
            return hash.ToHashCode();
        }

        public static bool operator ==(OnDataChangedAttributeData left, OnDataChangedAttributeData right) => left.Equals(right);
        public static bool operator !=(OnDataChangedAttributeData left, OnDataChangedAttributeData right) => !left.Equals(right);

    }
}

