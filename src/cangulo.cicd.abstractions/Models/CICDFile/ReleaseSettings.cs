namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class ReleaseSettings
    {
        public string CurrentVersion { get; set; }
        public string[] ReleaseAssets { get; set; }
        public UpdateVersionInCSProjSettings UpdateVersionInCSProjSettings { get; set; }
        public GitPushFiles GitPushReleaseFilesSettings { get; set; }
    }

    public class GitPushFiles
    {
        public string[] FoldersPath { get; set; }
        public string[] FilesPath { get; set; }
    }
}