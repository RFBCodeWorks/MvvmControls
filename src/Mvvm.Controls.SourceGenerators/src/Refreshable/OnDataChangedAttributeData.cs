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

        /// <summary>
        /// True if any of the commands/actions have data. False if all are default or empty.
        /// </summary>
        public bool HasData =>
            !SelectorActionsToInvoke.IsDefaultOrEmpty ||
            !CommandsToNotify.IsDefaultOrEmpty ||
            !ActionsToInvoke.IsDefaultOrEmpty;

        public OnDataChangedAttributeData(ImmutableArray<string> actions, ImmutableArray<string> commands, ImmutableArray<string> selectorActions)
        {
            ActionsToInvoke = actions;
            CommandsToNotify = commands;
            SelectorActionsToInvoke = selectorActions;
        }

        public bool Equals(OnDataChangedAttributeData other) =>
            CommandsToNotify.SequenceEqual(other.CommandsToNotify)
            && ActionsToInvoke.SequenceEqual(other.ActionsToInvoke);

        public override bool Equals(object obj) => obj is OnDataChangedAttributeData other && Equals(other);

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var cmd in CommandsToNotify) hash.Add(cmd);
            foreach (var cmd in ActionsToInvoke) hash.Add(cmd);
            foreach (var cmd in SelectorActionsToInvoke) hash.Add(cmd);
            return hash.ToHashCode();
        }

        public static bool operator ==(OnDataChangedAttributeData left, OnDataChangedAttributeData right) => left.Equals(right);
        public static bool operator !=(OnDataChangedAttributeData left, OnDataChangedAttributeData right) => !left.Equals(right);

    }
}

