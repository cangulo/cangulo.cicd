using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions(
    "PR_Execute_DOTNET_UT",
    GitHubActionsImage.UbuntuLatest,
    OnPullRequestIncludePaths = new[] { "src/**" },
    InvokedTargets = new[] { nameof(ExecuteUnitTests) })]
[GitHubActions(
    "PR_MERGED_RELEASE_NEW_VERSION",
    GitHubActionsImage.UbuntuLatest,
    OnPushBranches = new[] { "main" },
    InvokedTargets = new[] {
        nameof(ExecuteUnitTests),
        nameof(Publish),
        nameof(UpdateVersionInFiles) },
    ImportGitHubTokenAs = nameof(GitHubToken)
    )]
internal partial class Build : NukeBuild { }