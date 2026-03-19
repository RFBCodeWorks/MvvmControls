using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RFBCodeWorks.Mvvm.Tests.TestHelpers
{
    internal static class SourceGeneratorHelpers
    {

#if ROSLYN_311
        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator
        (this ISourceGenerator generator, string inputSource)
            => RunSourceGenerator(inputSource, out var _, generator);

        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator
        (this ISourceGenerator generator, string inputSource, out GeneratorDriverRunResult runResult)
            => RunSourceGenerator(inputSource, out runResult,  generator);
        
        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator
        (string inputSource, params ISourceGenerator[] generators)
            => RunSourceGenerator(inputSource, out var _, generators);

        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator(
            string inputSource, 
            out GeneratorDriverRunResult runResult, 
            params ISourceGenerator[] generators
            )
        {
            var compilation = CreateCompilation(inputSource);

            var driver = CSharpGeneratorDriver.Create(generators);
            driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            runResult = driver.GetRunResult();

            return (outputCompilation, diagnostics);
        }
#else
        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator(this IIncrementalGenerator generator, string inputSource)
            => RunSourceGenerator(inputSource, out _, generator);

        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator(
            string inputSource,
            params IIncrementalGenerator[] generators)
            => RunSourceGenerator(inputSource, out var _, generators);

        public static (Compilation compilation, ImmutableArray<Diagnostic> diagnostics) RunSourceGenerator(
            string inputSource,
            out GeneratorDriverRunResult runResult,
            params IIncrementalGenerator[] generators)
        {
            var compilation = CreateCompilation(inputSource);

            var driver = CSharpGeneratorDriver.Create(generators);
            driver = (CSharpGeneratorDriver)driver.RunGeneratorsAndUpdateCompilation(
                compilation,
                out var outputCompilation,
                out var diagnostics);

            runResult = driver.GetRunResult();

            return (outputCompilation, diagnostics);
        }
#endif

        static void AddMetadataReference(this List<MetadataReference> list, string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            if (!string.IsNullOrWhiteSpace(assembly.Location))
            {
                list.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }
        static void AddMetadataReference<T>(this List<MetadataReference> list)
        {
            var location = typeof(T).Assembly.Location;
            if (!string.IsNullOrWhiteSpace(location))
            {
                list.Add(MetadataReference.CreateFromFile(location));
            }
        }

        private static Compilation CreateCompilation(string inputSource)
        {
            SyntaxTree[] syntaxTrees = 
            [
                CSharpSyntaxTree.ParseText(inputSource)
            ];

            var references = new List<MetadataReference>();

            // Add all references from the current app domain's loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic && !string.IsNullOrWhiteSpace(assembly.Location))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }
            references.AddMetadataReference<RFBCodeWorks.Mvvm.ButtonAttribute>();
            references.AddMetadataReference<CommunityToolkit.Mvvm.ComponentModel.ObservableObject>();

            try
            {
                return CSharpCompilation.Create(
                    assemblyName: "TestAssembly",
                    syntaxTrees: syntaxTrees,
                    references: references,
                    options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    );
            }
            catch (Exception ex)
            {
                throw new AssertFailedException($"Failed to create compilation. \n\n{ex.Message}", ex);
            }
        }

        private static string? NormalizeTreeForComparison(string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return code;

            // Remove comments
            code = Regex.Replace(code, @"//.*$", "", RegexOptions.Multiline);
            code = Regex.Replace(code, @"/\*.*?\*/", "", RegexOptions.Singleline);

            // Remove attributes
            code = Regex.Replace(code, @"^\s*\[[^()]+(\([^()]*\))?\]", "", RegexOptions.Multiline);

            // remove pragmas
            code = Regex.Replace(code, @"^\s*#.*", "", RegexOptions.Multiline);

            // Collapse content within parentheses to single line
            code = Regex.Replace(code, @"\(([^)]*)\)", m =>
            {
                var content = m.Groups[1].Value;
                // Replace newlines and multiple spaces with single space
                content = Regex.Replace(content, @"\s+", " ").Trim();
                return $"({content})";
            }, RegexOptions.Singleline);

            // Collapse newlines between identifier and opening parenthesis
            code = Regex.Replace(code, @"(\w+)\s*\n?\s*\(", "$1(", RegexOptions.Multiline);

            code = Regex.Replace(code, @"(\w+)\s*\n\s*{", "$1 {\n\t", RegexOptions.Multiline);

            // Normalize whitespace while preserving structure
            var lines = code.Split('\n')
                .Select(l => l.TrimEnd().Replace("\t", "    "))
                .Where(l => !string.IsNullOrWhiteSpace(l));

            return string.Join("\n", lines);
        }

        public static void AssertNormalizedTreeEquals(this SyntaxTree generatedTree, string expected) => AssertNormalizedTreeEquals(generatedTree?.ToString(), expected);
        public static void AssertNormalizedTreeEquals(string? actual, string expected)
        {
            if (string.IsNullOrWhiteSpace(actual))
            {
                throw new AssertFailedException("Generated syntax tree is null or empty.");
            }
            if (string.IsNullOrWhiteSpace(expected))
            {
                throw new AssertFailedException("INVALID TEST SETUP - EXPECTED TREE IS NULL OR EMPTY.");
            }

            var normalizedExpected = NormalizeTreeForComparison(expected);
            var normalizedActual = NormalizeTreeForComparison(actual);

            try
            {
                Assert.AreEqual(normalizedExpected, normalizedActual, "Normalized syntax trees do not match.");
            }
            catch (AssertFailedException)
            {
                const string normalizedPrintformat = "\n\n------------- {0} --------------\n{1}";
                Console.WriteLine(normalizedPrintformat, "NORMALIZED EXPECTED", normalizedExpected);
                Console.WriteLine(normalizedPrintformat, "NORMALIZED GENERATED", normalizedActual);
                Console.WriteLine("\n\n-------------------------------------------\n");
                throw new AssertFailedException("Normalized syntax trees do not match.");
            }
        }

        public static void AssertNoDiagnostics(this ImmutableArray<Diagnostic> diagnostics)
        {
            var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
            try
            {
                Assert.AreEqual(0, errors.Length, $"Expected no diagnostics");
            }
            catch
            {
                Console.WriteLine("Diagnostics:");
                errors.PrintDiagnostics();
                throw;
            }
        }

        public static void AssertHasDiagnostic(this ImmutableArray<Diagnostic> diagnostics, string diagnosticId)
        {
            var matchingDiagnostics = diagnostics.Where(d => d.Id == diagnosticId).ToArray();
            Assert.IsTrue(matchingDiagnostics.Length > 0,
                $"Expected to find diagnostic with ID '{diagnosticId}', but none were found.");
        }

        public static void AssertGeneratedTreeFound(this SyntaxTree? generatedTree)
        {
            Assert.IsTrue(generatedTree is not null, "Expected a generated syntax tree, but it was null.");
        }

        public static void AssertCompilationHasNoErrors(this Compilation compilation)
        {
            var compilationDiagnostics = compilation.GetDiagnostics();
            var errors = compilationDiagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();

            try
            {
                Assert.AreEqual(0, errors.Length, $"Compilation failed with errors");
            }
            catch (AssertFailedException)
            {
                Console.WriteLine("Compilation Errors:");
                foreach (var diag in errors)
                {
                    Console.WriteLine(diag.ToString());
                }
                throw;
            }
        }

        public static void AssertHasDiagnostic(this Compilation compilation, string diagnosticID)
        {
            var compilationDiagnostics = compilation.GetDiagnostics();

            var diags = compilationDiagnostics.Where(d => d.Id == diagnosticID).ToArray();
            Assert.IsTrue(0 < diags.Length, $"Diagnostic ID {{{diagnosticID}}} was not found.");
        }

        public static INamedTypeSymbol AssertGetTypeByName(this Compilation compilation, string typeName)
        {
            var typeSymbol = compilation.GetTypeByMetadataName(typeName);
            Assert.IsNotNull(typeSymbol, $"Type '{typeName}' not found in compilation.");
            return typeSymbol;
        }

        public static void AssertOnConstructedMethod(this INamedTypeSymbol classSymbol)
        {
            // Verify OnConstructed partial method exists
            var onConstructedMethod = classSymbol.GetMembers("OnConstructed")
                .OfType<IMethodSymbol>()
                .FirstOrDefault();

            Assert.IsNotNull(onConstructedMethod, "OnConstructed method not found");
            Assert.IsTrue(onConstructedMethod.IsPartialDefinition || onConstructedMethod.PartialImplementationPart != null, "OnConstructed should be a partial method");
            Assert.AreEqual(0, onConstructedMethod.Parameters.Length, "OnConstructed should have no parameters");
            Assert.AreEqual(SpecialType.System_Void, onConstructedMethod.ReturnType.SpecialType, "OnConstructed should return void");
        }

        /// <summary>
        /// Gets the first generated syntax tree from the compilation, which is expected to be the output of the source generator.
        /// </summary>
        /// <param name="compilation"></param>
        /// <returns></returns>
        public static SyntaxTree? GetGeneratedTree(this Compilation compilation)
        {
            // Get generated source tree
            var generatedTree = compilation.SyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith(".g.cs"));
            return generatedTree;
        }

        /// <summary>
        /// Gets the first generated syntax tree from the compilation that matches the specified class name, which is expected to be the output of the source generator.
        /// </summary>
        public static SyntaxTree? GetGeneratedTree(this Compilation compilation, string className)
        {
            // Get generated source tree
            var generatedTree = compilation.SyntaxTrees.FirstOrDefault(t => t.FilePath.EndsWith($"{className}.g.cs"));
            generatedTree ??= compilation.SyntaxTrees.FirstOrDefault(t => t.FilePath.Contains(className));
            if (generatedTree is null)
            {
                Console.WriteLine($"\n\nWarning : Unable to locate generated tree with class name : {className}");
            }
            return generatedTree;
        }

        public static SyntaxTree? PrintGeneratedTree(this Compilation compilation) => GetGeneratedTree(compilation)?.PrintGeneratedTree();
        public static SyntaxTree? PrintGeneratedTree(this Compilation compilation, string className) => GetGeneratedTree(compilation, className)?.PrintGeneratedTree();

        public static void PrintAllGeneratedTrees(this Compilation compilation)
        {
            foreach(var tree in compilation.SyntaxTrees.Where(t => t.FilePath.EndsWith(".g.cs")))
            {
                Console.WriteLine("\n\n -------------------- Generated Tree : " + tree.FilePath + " -------------------- \n\n");
                tree.PrintGeneratedTree();
            }
        }

        public static SyntaxTree? PrintGeneratedTree(this SyntaxTree generatedTree)
        {
            // Print generated source to console
            if (generatedTree != null)
            {
                var generatedText = generatedTree.GetText().ToString();
                Console.WriteLine("\n\n=== Generated Source Code (Injected Required=true) ===");
                Console.WriteLine(generatedText);
                Console.WriteLine("=== End Generated Source ===");
            }
            else
            {
                Console.WriteLine("\n\nWARNING: No generated source file found!");
            }
            return generatedTree;
        }

        public static void PrintDiagnostics(this IEnumerable<Diagnostic> diagnostics)
        {
            if (diagnostics.Any())
            {
                Console.WriteLine("\n\n=== Diagnostics ===");
                foreach (var diag in diagnostics)
                {
                    Console.WriteLine($"{diag.Severity}: {diag.GetMessage()} (ID: {diag.Id})");
                }
                Console.WriteLine("=== End Diagnostics ===\n\n");
            }
            else
            {
                Console.WriteLine("\n\nNo diagnostics reported.\n\n");
            }
        }
    }
}
