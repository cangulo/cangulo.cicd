using System.Linq;
using System.Text.Json;
using cangulo.cicd.Abstractions.Constants;
using cangulo.cicd.domain.Extensions;
using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Octokit;

internal partial class Build : NukeBuild
{
    private Target SetupGitInPipeline => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
        {
            ValidateCICDPropertyIsProvided(CICDFile.GitPipelineSettings, nameof(CICDFile.GitPipelineSettings));

            var request = CICDFile.GitPipelineSettings;
            Logger.Info("Setting email and name in git");

            Git($"config --global user.email \"{request.Email}\"");
            Git($"config --global user.name \"{request.Name}\"");
        });

    private Target GitPush => _ => _
        .DependsOn(ParseCICDFile, SetupGitInPipeline)
        .Executes(() =>
        {
            Git($"add cicd.json", logOutput: true);
            Git($"commit -m \"[ci] new version {CICDFile.VersioningSettings.CurrentVersion} created\"", logOutput: true);
            Git($"push", logOutput: false);
        });

    private Target GetLastConventionalCommit => _ => _
        .Executes(() =>
        {
            var commitParser = _serviceProvider.GetRequiredService<ICommitParser>();

            var lastCommitMsg = Git($"log --format=%B -n 1", logOutput: true).ConcatenateOutputText();
            var conventionalCommit = commitParser.ParseConventionalCommit(lastCommitMsg);

            Logger.Info($"ConventionCommit:\n{JsonSerializer.Serialize(conventionalCommit, SerializerContants.SERIALIZER_OPTIONS)}");
        });

    private Target GetCommitsInAMergedPR => _ => _
        .Executes(async () =>
        {
            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);

            // TODO: Migrate the injection of the client to an interface
            var ghClient = new GitHubClient(new ProductHeaderValue($"{repoOwner}"));
            ghClient.Credentials = new Credentials(GitHubToken);
            var client = ghClient.Repository.PullRequest;

            var query = new PullRequestRequest
            {
                State = ItemStateFilter.Closed,
                Base = GitHubActions.GitHubBaseRef,
                Head = GitHubActions.GitHubHeadRef,
                SortProperty = PullRequestSort.Updated
            };
            var apiOptions = new ApiOptions { PageCount = 1, PageSize = 1 };
            var prMerged = (await client.GetAllForRepository(repoOwner, repoName, query, apiOptions)).Single();

            Logger.Info($"PR:\n{JsonSerializer.Serialize(prMerged, SerializerContants.SERIALIZER_OPTIONS)}");

            var commits = await client.Commits(repoOwner, repoName, prMerged.Number);
            Logger.Info($"Commits found:{commits.Count}");
            commits
                .ToList()
                .ForEach(x =>
                    Logger.Info($"{x.Commit.Sha}: {x.Commit.Message}"));

        });
}