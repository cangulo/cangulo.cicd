using System.ComponentModel;
using Nuke.Common.Tooling;

namespace cangulo.build.Build.Parameters
{
    [TypeConverter(typeof(TypeConverter<Configuration>))]
    public class NugetPackageCreationMode : Enumeration
    {
        public static NugetPackageCreationMode Prerelease = new NugetPackageCreationMode { Value = nameof(Prerelease) };
        public static NugetPackageCreationMode Release = new NugetPackageCreationMode { Value = nameof(Release) };

        public static implicit operator string(NugetPackageCreationMode configuration)
        {
            return configuration.Value;
        }
    }
}
