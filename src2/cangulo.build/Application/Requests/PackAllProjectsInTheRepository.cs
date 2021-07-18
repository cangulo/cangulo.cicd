using cangulo.build.Application.Requests.Enums;

namespace cangulo.build.Application.Requests
{
    public class PackAllProjectsInTheRepository : CLIRequest
    {
        public NugetPackModeEnum CreationMode { get; set; }
        public string OutputFolder { get; set; }
        public string Branch { get; set; }
    }
}