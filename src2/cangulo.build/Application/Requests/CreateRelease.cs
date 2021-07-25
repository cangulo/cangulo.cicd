using cangulo.build.Application.Requests.Enums;

namespace cangulo.build.Application.Requests
{
    public class CreateRelease : CLIRequest
    {
        public long RepositoryId { get; set; }
        public string Tag { get; set; }
        public string Title { get; set; }
        public string ReleaseNotesFilePath { get; set; }
        public string ReleaseAssetsFolder { get; set; }

        public static new EnvVar[] EnvVarsRequired =
            new EnvVar[] {
                EnvVar.GITHUB_TOKEN
            };

    }
}