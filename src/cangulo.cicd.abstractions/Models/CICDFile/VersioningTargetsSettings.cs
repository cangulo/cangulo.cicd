namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class VersioningTargetsSettings
    {
        public string CurrentVersion { get; set; }
        public string[] ReleaseAssets { get; set; }
        public UpdateVersionInCSProjSettings UpdateVersionInCSProjSettings { get; set; }
    }
}