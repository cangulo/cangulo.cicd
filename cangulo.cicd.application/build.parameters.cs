using cangulo.cicd.Abstractions.Requests;
using Nuke.Common;
using Nuke.Common.ProjectModel;

internal partial class Build : NukeBuild
{
    public CICDFileModel CICDFile;
    public Solution TargetSolutionParsed;
}