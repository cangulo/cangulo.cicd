namespace cangulo.build.Application.Requests
{
    public class ExecuteUnitTests : CLIRequest
    {
        public string[] Solutions { get; set; }
    }
}