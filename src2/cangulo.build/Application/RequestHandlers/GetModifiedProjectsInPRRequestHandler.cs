using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using FluentResults;
using Nuke.Common;
using Nuke.Common.ProjectModel;
using Octokit;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static cangulo.build.abstractions.Constants;

namespace cangulo.build.Application.RequestHandlers
{
    public class GetModifiedProjectsInPRRequestHandler : ICLIRequestHandler<GetModifiedProjectsInPR, string[]>
    {
        private readonly INukeLogger _nukeLogger;
        private readonly BuildContext _buildContext;

        public GetModifiedProjectsInPRRequestHandler(INukeLogger nukeLogger, BuildContext buildContext)
        {
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
        }

        public async Task<Result<string[]>> Handle(GetModifiedProjectsInPR request, CancellationToken cancellationToken)
        {
            var githubToken = EnvironmentInfo.GetVariable<string>(EnvVar.GITHUB_TOKEN.ToString());

            var client = new GitHubClient(new ProductHeaderValue(request.Originator));
            client.Credentials = new Credentials(githubToken);

            var pr = await client.PullRequest.Get(request.RepositoryId, request.PullRequestNumber);
            _nukeLogger.Info($"PR Info:\nRepository: {pr.Head.Repository.Name}\nsourceBranch: {pr.Head.Ref}\ntargetBranch: {pr.Base.Ref}\n");

            var filesPR = await client.PullRequest.Files(request.RepositoryId, pr.Number);
            var filesModified = filesPR.Where(x => x.Status != "removed").Select(x => x.FileName);

            var projectsPackagable = _buildContext.Projects.Where(x => x.GetProperty<string>(CSProjProperties.VERSION_PREFIX) != null);

            _nukeLogger.Info($"Projects Modified:\n");
            var projectsModified = projectsPackagable.Where(x => filesModified.Any(y => y.Contains(x.Name))).Select(x => x.Name);
            projectsModified.ToList().ForEach(x => _nukeLogger.Info(x));

            if (!projectsModified.Any())
                _nukeLogger.Warn($"No project have been modified!");

            return Result.Ok(projectsModified.ToArray());
        }
    }
}