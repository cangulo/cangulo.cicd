using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Octokit;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;

namespace cangulo.cicd.domain.Services
{
    public interface IPullRequestService
    {
        Task<IEnumerable<string>> GetCommitsFromLastMergedPR(GitHubClient ghClient, GitHubActions gitHubActions);
        Task<IEnumerable<string>> GetCommitsFromCurrentPR(GitHubClient ghClient, GitHubActions gitHubActions);
    }
    public class PullRequestService : IPullRequestService
    {
        public async Task<IEnumerable<string>> GetCommitsFromCurrentPR(GitHubClient ghClient, GitHubActions gitHubActions)
        {
            ControlFlow.NotNull(gitHubActions, "This Target can't be executed locally");
            ControlFlow.NotNull(ghClient, "The GitHub client provided can't be null");

            var repoOwner = gitHubActions.GitHubRepositoryOwner;
            var repoName = gitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);

            var client = ghClient.Repository.PullRequest;

            var query = new PullRequestRequest
            {
                State = ItemStateFilter.Open,
                Base = gitHubActions.GitHubBaseRef,
                Head = gitHubActions.GitHubHeadRef,
                SortProperty = PullRequestSort.Updated
            };
            return await GetCommits(repoOwner, repoName, client, query);
        }

        public async Task<IEnumerable<string>> GetCommitsFromLastMergedPR(GitHubClient ghClient, GitHubActions gitHubActions)
        {
            ControlFlow.NotNull(gitHubActions, "This Target can't be executed locally");
            ControlFlow.NotNull(ghClient, "The GitHub client provided can't be null");

            var repoOwner = gitHubActions.GitHubRepositoryOwner;
            var repoName = gitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);

            var client = ghClient.Repository.PullRequest;

            var query = new PullRequestRequest
            {
                State = ItemStateFilter.Closed,
                Base = gitHubActions.GitHubBaseRef,
                Head = gitHubActions.GitHubHeadRef,
                SortProperty = PullRequestSort.Updated
            };
            return await GetCommits(repoOwner, repoName, client, query);
        }

        private static async Task<IEnumerable<string>> GetCommits(string repoOwner, string repoName, IPullRequestsClient client, PullRequestRequest query)
        {
            var apiOptions = new ApiOptions { PageCount = 1, PageSize = 1 };
            var prs = (await client.GetAllForRepository(repoOwner, repoName, query, apiOptions));

            if (prs.Count() == 0)
                throw new Exception("No PR found");

            Logger.Info($"pr number: {prs.Single().Number}");

            var commits = await client.Commits(repoOwner, repoName, prs.Single().Number);
            return commits.Select(x => x.Commit.Message);
        }
    }
}
