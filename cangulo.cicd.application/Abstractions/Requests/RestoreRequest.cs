namespace cangulo.cicd.Abstractions.Requests
{
    public class RestoreRequest : BaseRequest
    {
        public string SolutionPath { get; set; }
    }

    public class CompileRequest : BaseRequest
    {
        public string SolutionPath { get; set; }
    }
}
