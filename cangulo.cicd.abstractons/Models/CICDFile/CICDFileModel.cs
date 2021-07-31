namespace cangulo.cicd.abstractons.Models.CICDFile
{
    public class CICDFileModel
    {
        public string SolutionPath { get; set; }
        public DotnetPublishSettings DotnetPublish { get; set; }
        public CompressDirectory CompressDirectory { get; set; }
        public VersioningSettings VersioningSettings { get; set; }
        public GitPipelineSettings GitPipelineSettings { get; set; }
    }
}
