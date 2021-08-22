using cangulo.cicd.abstractions.Models.CICDFile;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using System.Collections.Generic;

internal partial class Build : NukeBuild
{
    public CICDFileModel CICDFile;

    public AbsolutePath CICDFilePath = RootDirectory / "cicd.json";

    public Solution TargetSolutionParsed;

    [GitRepository]
    private readonly GitRepository GitRepository;

    [PathExecutable("git")]
    private readonly Tool Git;

    [PathExecutable("zip")]
    private readonly Tool Zip;

    [Parameter("GitHub auth token", Name = "github-token"), Secret]
    private readonly string GitHubToken;

    [CI] GitHubActions GitHubActions;

    public IDictionary<string, object> ResultBagDictionary;
    public AbsolutePath ResultBagFilePath = RootDirectory / "cicd.resultbag.json";
}