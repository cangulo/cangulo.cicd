using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions("execute_ut_on_push", GitHubActionsImage.UbuntuLatest, OnPushIncludePaths = new string[] { "./src" }, InvokedTargets = new[] { nameof(ExecuteUnitTests) })]
internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Publish);
}