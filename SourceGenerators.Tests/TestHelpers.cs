using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RFBCodeWorks.Mvvm.SourceGenerators.Tests
{
    internal static class TestHelpers
    {
        public static Compilation CreateCompilation(string source, string assemblyName = "TestAssembly")
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(source);
            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Assert).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(RFBCodeWorks.Mvvm.SourceGenerators.ButtonGeneratorRoslyn40).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(RFBCodeWorks.Mvvm.ButtonDefinition).Assembly.Location)
            };
            return CSharpCompilation.Create(
                assemblyName,
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        }

        public static (GeneratorDriver Driver, Compilation OutputCompilation, ImmutableArray<Diagnostic> Diagnostics) RunGenerator(this Compilation inputCompilation, params ISourceGenerator[] generators)
        {
            var driver = CSharpGeneratorDriver.Create(generators);
            driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
            return (driver, outputCompilation, diagnostics);
        }

        public static void AssertNoDiagnostics(this Compilation compilation)
        {
            var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
            if (errors.Any())
                throw new AssertFailedException($"Expected no errors, but found: {string.Join(", ", errors)}");
        }

        public static void AssertFieldExists(this Compilation compilation, string className, string fieldName, string fieldType)
        {
            var tree = compilation.SyntaxTrees.Last();
            var model = compilation.GetSemanticModel(tree);
            var root = tree.GetRoot();
            var fieldDecl = root.DescendantNodes()
                .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax>()
                .FirstOrDefault(f => f.Declaration.Variables.Any(v => v.Identifier.Text == fieldName) &&
                                     model.GetTypeInfo(f.Declaration.Type).Type.ToDisplayString() == fieldType);
            if (fieldDecl == null)
                throw new AssertFailedException($"Field '{fieldName}' of type '{fieldType}' not found.");
        }

        public static void AssertPropertyInitialization(this Compilation compilation, string className, string propertyName, Regex initializerPattern)
        {
            var tree = compilation.SyntaxTrees.Last();
            var root = tree.GetRoot();
            var propDecl = root.DescendantNodes()
                .OfType<Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax>()
                .FirstOrDefault(p => p.Identifier.Text == propertyName);
            if (propDecl == null)
                throw new AssertFailedException($"Property '{propertyName}' not found.");
            var text = propDecl.ToFullString();
            if (!initializerPattern.IsMatch(text))
                throw new AssertFailedException($"Initializer pattern '{initializerPattern}' not matched in property '{propertyName}'.");
        }
    }
}