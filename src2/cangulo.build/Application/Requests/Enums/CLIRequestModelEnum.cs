namespace cangulo.build.Application.Requests.Enums
{
    public enum CLIRequestModelEnum
    {
        Undefined,
        ExecuteUnitTests,
        PackProjects,
        PushNugetPackages,
        ExecuteAllUnitTestsInTheRepository,
        PackAllProjectsInTheRepository,
        GetModifiedProjectsInPR,
        GetNumberLastPRMerged,
        GetCommitsActions,
        ProcessCommitActions,
        CreateRelease
    }
}