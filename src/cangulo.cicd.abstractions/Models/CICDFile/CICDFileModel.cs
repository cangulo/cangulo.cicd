using cangulo.changelog.Models;
using System.Text.Json.Serialization;

namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class CICDFileModel
    {
        [JsonPropertyName("$schema")]
        public string Schema { get; set; }
        public DotnetTargetsSettings DotnetTargets { get; set; }
        public CompressDirectoryTargetSettings CompressDirectory { get; set; }
        public VersioningTargetsSettings Versioning { get; set; }
        public GitTargetsSettings GitTargets { get; set; }
        public ChangelogSettings ChangelogSettings { get; set; }
    }
}
