using cangulo.build.abstractions.Models.Enums;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using cangulo.build.domain;
using FluentResults;
using Nuke.Common;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.build.Application.RequestHandlers
{
    public class GetCommitsActionsRequestHandler : ICLIRequestHandler<GetCommitsActions, IEnumerable<CommitAction>>
    {
        private readonly ICommitMessageService _commitMessageService;
        private readonly INukeLogger _nukeLogger;

        public GetCommitsActionsRequestHandler(ICommitMessageService commitMessageService, INukeLogger nukeLogger)
        {
            _commitMessageService = commitMessageService ?? throw new ArgumentNullException(nameof(commitMessageService));
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
        }

        public async Task<Result<IEnumerable<CommitAction>>> Handle(GetCommitsActions request, CancellationToken cancellationToken)
        {
            var githubToken = EnvironmentInfo.GetVariable<string>(EnvVar.GITHUB_TOKEN.ToString());

            var client = new GitHubClient(new ProductHeaderValue(request.Originator));
            client.Credentials = new Credentials(githubToken);

            var pr = await client.PullRequest.Get(request.RepositoryId, request.PullRequestNumber);
            _nukeLogger.Info($"PR Info:\nRepository: {pr.Head.Repository.Name}\nsourceBranch: {pr.Head.Ref}\ntargetBranch: {pr.Base.Ref}\n");

            var commits = await client.PullRequest.Commits(request.RepositoryId, request.PullRequestNumber);
            var commitMsgs = commits.Select(x => x.Commit.Message);

            var actions = _commitMessageService
                .GetActions(commitMsgs)
                .Where(x => x != CommitAction.Undefined);

            if (actions.Any())
            {
                _nukeLogger.Info($"Actions found:\n");
                actions.ToList().ForEach(x => _nukeLogger.Info($"\t{x}"));
            }
            else
                _nukeLogger.Info($"No CI actions provided in the commits");

            return Result.Ok(actions);
        }
    }
}