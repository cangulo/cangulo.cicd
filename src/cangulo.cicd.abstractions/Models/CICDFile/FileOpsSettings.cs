namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class FileOpsSettings
    {
        public CompressDirectorySettings CompressDirectorySettings { get; set; }
    }
    public class CompressDirectorySettings
    {
        public string Path { get; set; }
        public string OutputFileName { get; set; }
    }
}