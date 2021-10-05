namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class GitSettings
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public GitPushFiles GitPushReleaseFilesSettings { get; set; }

    }
    public class GitPushFiles
    {
        public string[] FoldersPath { get; set; }
        public string[] FilesPath { get; set; }
    }
}
