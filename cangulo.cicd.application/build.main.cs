using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions(
    "PR_Execute_DOTNET_UT",
    GitHubActionsImage.UbuntuLatest,
    OnPullRequestIncludePaths = new string[] { "src/**" },
    InvokedTargets = new[] { nameof(ExecuteUnitTests) })]
//[GitHubActions("propose_next_version", GitHubActionsImage.UbuntuLatest, OnPullRequestBranches = new string[] { "main" }, InvokedTargets = new[] { nameof(ProposeNextVersion) })]
internal partial class Build : NukeBuild
{
    // TODO: pipelines depending of the branch
    // 1. When a PR is created, execute the UT
    // 2. When a PR is created
    // 2. a.    Check the commits and decide what type of Release should be created
    // 2. b.    Create a release
    // 2. c.    Push the files of the new release
    public static int Main() => Execute<Build>(x => x.ProposeNextVersion);
}