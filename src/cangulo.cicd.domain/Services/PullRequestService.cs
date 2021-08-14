using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Octokit;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace cangulo.cicd.domain.Services
{
    public interface IPullRequestService
    {
        Task<IEnumerable<string>> GetCommitsFromLastMergedPR(GitHubClient ghClient, GitHubActions gitHubActions);
    }
    public class PullRequestService : IPullRequestService
    {
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
            var apiOptions = new ApiOptions { PageCount = 1, PageSize = 1 };
            var prMerged = (await client.GetAllForRepository(repoOwner, repoName, query, apiOptions)).Single();

            // Logger.Info($"PR:\n{JsonSerializer.Serialize(prMerged, SerializerContants.SERIALIZER_OPTIONS)}");

            var commits = await client.Commits(repoOwner, repoName, prMerged.Number);
            return commits.Select(x => x.Commit.Message);
        }
    }
}
