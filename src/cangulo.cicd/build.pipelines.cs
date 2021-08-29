using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
[GitHubActions(
    "PR_Execute_DOTNET_UT",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    OnPullRequestIncludePaths = new[] { "src/**", "test/**" },
    InvokedTargets = new[] { nameof(ExecuteUnitTests) })]
[GitHubActions(
    "PR_MERGED_RELEASE_NEW_VERSION",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = false,
    OnPushBranches = new[] { "main" },
    InvokedTargets = new[] {
        nameof(ExecuteUnitTests),
        nameof(Publish),
        nameof(UpdateVersionInCICDFile),
        nameof(CompressDirectory),
        nameof(CreateNewRelease)
        },
    ImportGitHubTokenAs = nameof(GitHubToken)
    )]
internal partial class Build : NukeBuild { }