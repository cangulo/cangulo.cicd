using cangulo.build.abstractions.Models;
using cangulo.build.abstractions.Models.Enums;
using cangulo.build.Application.Requests.Enums;
using System.Collections.Generic;

namespace cangulo.build.Application.Requests
{
    public class ProcessCommitActions : CLIRequest<ProgramVersion>
    {
        public IEnumerable<CommitAction> CommitActions { get; set; }

        public static new EnvVar[] EnvVarsRequired =
            new EnvVar[] {
                EnvVar.GITHUB_TOKEN,
                EnvVar.OUTPUT_FILE_PATH
            };
    }
}