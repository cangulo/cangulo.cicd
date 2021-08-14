using cangulo.cicd.abstractons.Models.CICDFile;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;

internal partial class Build : NukeBuild
{
    public CICDFileModel CICDFile;

    public Solution TargetSolutionParsed;

    [GitRepository]
    private readonly GitRepository GitRepository;

    [PathExecutable("git")]
    private readonly Tool Git;

    [Parameter("GitHub auth token", Name = "github-token"), Secret]
    private readonly string GitHubToken;

    [CI] GitHubActions GitHubActions;
}