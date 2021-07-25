using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);
}