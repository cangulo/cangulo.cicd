namespace cangulo.cicd.abstractons.Models.CICDFile
{
    public class CICDFileModel
    {
        public DotnetTargetsSettings DotnetTargets { get; set; }
        public CompressDirectoryTargetSettings CompressDirectory { get; set; }
        public VersioningTargetsSettings Versioning { get; set; }
        public GitTargetsSettings GitTargets { get; set; }
    }
}
