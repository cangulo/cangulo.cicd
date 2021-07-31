namespace cangulo.cicd.abstractons.Models.CICDFile
{
    public class VersioningSettings
    {
        public string CurrentVersion { get; set; }
        public string[] ReleaseAssets { get; set; }
    }
}
