using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable
#nullable disable warnings

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
                ?.ConstructorArguments.FirstOrDefault().Value is string tfm
                && TryParseTargetFrameworkMoniker(tfm, out var frameworkIdentifier, out var version))
            {
                switch (frameworkIdentifier)
                {
                    case ".NETFramework":
                        return TargetFramework.NetFramework;
                    case ".NETStandard":
                        if (version.Major == 2 && version.Minor == 0) return TargetFramework.NetStandard2_0;
                        if (version.Major == 2 && version.Minor == 1) return TargetFramework.NetStandard2_1;
                        break;
                    case ".NETCoreApp":
                        if (version.Major == 3 && version.Minor == 1) return TargetFramework.NetCore31;
                        if (version.Major == 5) return TargetFramework.Net5;
                        if (version.Major == 6) return TargetFramework.Net6;
                        if (version.Major == 7) return TargetFramework.Net7;
                        if (version.Major >= 8) return TargetFramework.Net8_OrNewer;
                        break;
                }
            }
            return TargetFramework.Unknown;
        }

        private static bool TryParseTargetFrameworkMoniker(string tfm, out string frameworkIdentifier, out Version version)
        {
            frameworkIdentifier = null;
            version = null;

            var match = Regex.Match(tfm, @"^(?<identifier>[^,]+),Version=v(?<major>\d+)(\.(?<minor>\d+))?$");
            if (!match.Success)
            {
                return false;
            }

            frameworkIdentifier = match.Groups["identifier"].Value;
            var major = int.Parse(match.Groups["major"].Value);
            var minorGroup = match.Groups["minor"];
            var minor = minorGroup.Success ? int.Parse(minorGroup.Value) : 0;
            version = new Version(major, minor);
            return true;
        }
    }

}
