namespace cangulo.cicd.abstractons.Models.CICDFile
{
    public class DotnetTargetsSettings
    {
        public string SolutionPath { get; set; }
        public DotnetPublishSettings DotnetPublish { get; set; }
    }
}
