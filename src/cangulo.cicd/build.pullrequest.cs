using System;
using System.Linq;
using cangulo.cicd.abstractions.Constants;
using cangulo.cicd.domain.Parsers;
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

            Logger.Info($"{commitMsgs.Count()} commits found:");
            commitMsgs
                .ToList()
                .ForEach(Logger.Success);

            var resultKey = nameof(ListCommitsInThisPR);
            resultBagRepository.AddResult(resultKey, commitMsgs);
        });

    private Target VerifyAllCommitsAreConventionalInThisPR => _ => _
        .Executes(async () =>
        {
            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();

            var ghClient = GetGHClient(GitHubActions);
            var commitMsgs = await prService.GetCommitsFromCurrentPR(ghClient, GitHubActions);

            ControlFlow.Assert(commitMsgs.Any(), "no commits founds");

            var CI_COMMIT_PREFIX = CiActionsContants.CI_ACTION_COMMIT_PREFIX;

            var commits = commitMsgs
                                    .Where(x => !x.StartsWith(CI_COMMIT_PREFIX))
                                    .ToList();

            Logger.Info($"{commits.Count} commits found:");
            commits
                .ForEach(Logger.Info);

            try
            {
                var conventionalCommits = commits
                    .Select(commitParser.ParseConventionalCommit)
                    .ToList();

                Logger.Info($"{conventionalCommits.Count} conventional commits found:");
                conventionalCommits
                    .ForEach(Logger.Success);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error trying to parse commits:{ex}");
            }
        });
}