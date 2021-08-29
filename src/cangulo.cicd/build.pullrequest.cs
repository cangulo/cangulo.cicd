using System.Linq;
using cangulo.cicd.domain.Repositories;
using cangulo.cicd.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;

internal partial class Build : NukeBuild
{
    private Target ListCommitsInThisPR => _ => _
    .Executes(async () =>
    {
        var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
        var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();

        var ghClient = GetGHClient(GitHubActions);
        var commitMsgs = await prService.GetCommitsFromLastMergedPR(ghClient, GitHubActions);

        ControlFlow.Assert(commitMsgs.Any(), "no commits founds");

        Logger.Info($"Commits Found:{commitMsgs.Count()}");
        commitMsgs
            .ToList()
            .ForEach(Logger.Info);

        var resultKey = nameof(ListCommitsInThisPR);
        resultBagRepository.AddResult(resultKey, commitMsgs);
    });
}