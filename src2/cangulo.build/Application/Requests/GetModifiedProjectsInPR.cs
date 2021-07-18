using cangulo.build.Application.Requests.Enums;

namespace cangulo.build.Application.Requests
{
    public class GetModifiedProjectsInPR : CLIRequest<string[]>
    {
        public int PullRequestNumber { get; set; }
        public long RepositoryId { get; set; }
        public static new EnvVar[] EnvVarsRequired = new EnvVar[] { EnvVar.GITHUB_TOKEN, EnvVar.OUTPUT_FILE_PATH };
    }
}