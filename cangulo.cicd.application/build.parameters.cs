using cangulo.cicd.Abstractions.Requests;
using Nuke.Common;
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
}