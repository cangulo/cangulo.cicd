using cangulo.changelog.Models;
using System.Text.Json.Serialization;

namespace cangulo.cicd.abstractions.Models.CICDFile
{
    public class CICDFileModel
    {
        [JsonPropertyName("$schema")]
        public string Schema { get; set; }
        public DotnetSettings DotnetSettings { get; set; }
        public FileOpsSettings FileOpsSettings { get; set; }
        public ReleaseSettings ReleaseSettings { get; set; }
        public GitSettings GitSettings { get; set; }
        public ChangelogSettings ChangelogSettings { get; set; }
        public NugetSettings NugetSettings { get; set; }
    }
}
