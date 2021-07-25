namespace cangulo.cicd.Abstractions.Requests
{
    public class CICDFileModel
    {
        public string SolutionPath { get; set; }
        public DotnetPublishSettings DotnetPublish { get; set; }
    }

    public class DotnetPublishSettings
    {
        public string ProjectPath { get; set; }
        public string OutputFolder { get; set; }
        public string RunTime { get; set; }
        
        //= "-r linux-x64 --self-contained"
        //public bool SelfContained { get; set; }
    }
}
