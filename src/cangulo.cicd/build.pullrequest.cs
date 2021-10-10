using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            ControlFlow.NotNull(CICDFile.PullRequestSettings);
            var request = CICDFile.PullRequestSettings;

            var prService = _serviceProvider.GetRequiredService<IPullRequestService>();
            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();

            var ghClient = GetGHClient(GitHubActions);
            var commitMsgs = await prService.GetCommitsFromCurrentPR(ghClient, GitHubActions);

            ControlFlow.Assert(commitMsgs.Any(), "no commits founds");

            var commits = commitMsgs.ToList();

            Logger.Info($"{commits.Count} commits found:");
            commits
                .ForEach(Logger.Info);

            #region Validating Conventional Commits
            try
            {
                ControlFlow.NotEmpty(
                    request.ConventionalCommitsSettings,
                    "Please provide the conventional commit settings");
                var commitTypesAllowed = CICDFile
                                           .PullRequestSettings
                                           .ConventionalCommitsSettings
                                           .Select(x => x.Type)
                                           .ToArray();

                var conventionalCommits = commits
                    .Select(
                        comMsg => commitParser.ParseConventionalCommit(comMsg, commitTypesAllowed))
                    .ToList();

                Logger.Info($"{conventionalCommits.Count} conventional commits found:");
                conventionalCommits
                    .ForEach(Logger.Success);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error trying to parse commits:{ex}");
            }
            #endregion

            #region Validating Issue Numbers

            if (!string.IsNullOrEmpty(request.IssueNumberRegex))
            {
                commits.ForEach(x =>
                {

                    var issueProvided = Regex.IsMatch(x, request.IssueNumberRegex);
                    ControlFlow.Assert(issueProvided, $"issue number not provided for the commit {issueProvided}");
                });
            }

            #endregion

            #region Output Commit List

            if (request.OutputCommits)
                File.WriteAllLines(request.OutputFilePath, commitMsgs);

            #endregion
        });
}