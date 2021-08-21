namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class DotnetTargetsSettings
    {
        public string SolutionPath { get; set; }
        public DotnetPublishSettings DotnetPublish { get; set; }
    }
}
