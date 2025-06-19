using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;

namespace RFBCodeWorks.Mvvm.SourceGenerators.src
{
    internal enum TargetFramework
    {
        Unknown,
        NetStandard2_0,
        NetFramework,
        NetStandard2_1,
        NetCore31,
        Net5,
        Net6,
        Net7,
        Net8_OrNewer
    }

    internal readonly struct CompilationData : IEquatable<CompilationData>
    {
        public readonly LanguageVersion LanguageVersion;
        public readonly TargetFramework TargetFramework;
        public readonly Func<string, INamedTypeSymbol?> GetTypeByMetadataName;
        public CompilationData(LanguageVersion version, TargetFramework framework, Func<string, INamedTypeSymbol?> getTypeByMetadataName) { LanguageVersion = version; TargetFramework = framework; GetTypeByMetadataName = getTypeByMetadataName; }
        public bool Equals(CompilationData other) => LanguageVersion == other.LanguageVersion && TargetFramework == other.TargetFramework;
        public static CompilationData GetData(Compilation compilation, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (compilation is not CSharpCompilation cs)
            {
                return default;
            }
            else
            {
                var target = GetTargetFramework(compilation);
                return new CompilationData(cs.LanguageVersion, target, compilation.GetTypeByMetadataName);
            }
        }

        private static TargetFramework GetTargetFramework(Compilation compilation)
        {
            if (compilation.Assembly
                .GetAttributes()
                .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == "System.Runtime.Versioning.TargetFrameworkAttribute")
                ?.ConstructorArguments.FirstOrDefault().Value is string tfm)
            {
                if (tfm.Contains("NETFramework"))
                    return TargetFramework.NetFramework;

                switch (tfm)
                {
                    case ".NETStandard,Version=v2.0": return TargetFramework.NetStandard2_0;
                    case ".NETStandard,Version=v2.1": return TargetFramework.NetStandard2_1;
                    case ".NETCoreApp,Version=v3.1": return TargetFramework.NetCore31;
                    case ".NETCoreApp,Version=v5.0": return TargetFramework.Net5;
                    case ".NETCoreApp,Version=v6.0": return TargetFramework.Net6;
                    case ".NETCoreApp,Version=v7.0":
                        return TargetFramework.Net7;
                    case ".NETCoreApp,Version=v8.0": return TargetFramework.Net8_OrNewer;
                    default:
                        var match = Regex.Match(tfm, @"Version=v(?<major>\d+)(\.(?<minor>\d+))?");
                        if (match.Success)
                        {
                            int major = int.Parse(match.Groups["major"].Value);
                            if (major >= 8) return TargetFramework.Net8_OrNewer;
                            if (major == 7) return TargetFramework.Net7;
                            if (major == 3) return TargetFramework.NetStandard2_0;
                        }
                        break;
                }
            }
            return TargetFramework.Unknown;
        }
    }

}
