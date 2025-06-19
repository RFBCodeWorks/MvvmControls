using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Refreshable
{
    /// <summary>
    /// Helper for :
    /// <br/> OnSelectionChangedAttribute
    /// <br/> OnItemSourceChangedAttribute
    /// </summary>
    internal readonly struct OnDataChangedAttributeData : IEquatable<OnDataChangedAttributeData>
    {
        public const string QualifiedName_SelectionChanged = "RFBCodeWorks.Mvvm.OnSelectionChangedAttribute";
        public const string QualifiedName_ItemSourceChanged = "RFBCodeWorks.Mvvm.OnItemSourceChangedAttribute";
        
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
        public static OnDataChangedAttributeData GetItemSourceChanged(ISymbol symbol) => GetData(symbol, QualifiedName_ItemSourceChanged);
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
        public bool GenerateAnonymousMethod(SourceWriter writer, Action<Diagnostic> reportDiagnostic, string argumentName)
        {
            if (Diagnostic is not null)
            {
                reportDiagnostic?.Invoke(Diagnostic);
                return false;
            }
            if (this.CommandsToNotify.Length > 0 || this.MethodsToInvoke.Length > 0)
            {
                writer.Write(',').WriteLine().WriteLine($"{argumentName}: () => {{");
                writer.Indentation++;
                foreach (var cmd in CommandsToNotify)
                {
                    writer.WriteLine("{0}.NotifyCanExecuteChanged();", cmd);
                }
                foreach (var cmd in MethodsToInvoke)
                {
                    writer.WriteLine("{0}();", cmd);
                }
                writer.Indentation--;
                writer.WriteIndent().Write('}');
                return true;
            }
            return false;
        }
    }
}

