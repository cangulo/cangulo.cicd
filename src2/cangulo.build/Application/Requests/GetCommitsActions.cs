using cangulo.build.abstractions.Models.Enums;
using cangulo.build.Application.Requests.Enums;
using System.Collections.Generic;

namespace cangulo.build.Application.Requests
{
    public class GetCommitsActions : CLIRequest<IEnumerable<CommitAction>>
    {
        public int PullRequestNumber { get; set; }
        public long RepositoryId { get; set; }

        public static new EnvVar[] EnvVarsRequired =
            new EnvVar[] {
                EnvVar.GITHUB_TOKEN,
                EnvVar.OUTPUT_FILE_PATH
            };
    }
}