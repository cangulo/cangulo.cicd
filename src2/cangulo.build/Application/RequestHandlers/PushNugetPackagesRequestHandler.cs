using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using cangulo.build.Extensions;
using FluentResults;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Octokit;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.build.Application.RequestHandlers
{
    public class PushNugetPackagesRequestHandler : ICLIRequestHandler<PushNugetPackages>
    {
        private readonly BuildContext _buildContext;
        private readonly INukeLogger _nukeLogger;

        public PushNugetPackagesRequestHandler(BuildContext buildContext, INukeLogger nukeLogger)
        {
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
        }

        public async Task<Result> Handle(PushNugetPackages request, CancellationToken cancellationToken)
        {
            var nugetPackageAbsolutePath = _buildContext.RootDirectory / request.NugetPackagesLocation;
            var nuggets = nugetPackageAbsolutePath
                                    .GlobFiles("**/*.nupkg")
                                    .Select(x =>
                                    new
                                    {
                                        path = x.ToString(),
                                        fileName = Path.GetFileName(x.GetFileName())
                                    });

            foreach (var nuget in nuggets)
            {
                var publishSettings = new DotNetNuGetPushSettings()
                                .SetSource(request.TargetNugetRepository)
                                .SetTargetPath(nuget.path);

                _nukeLogger.Info($"Pushing Project {nuget.fileName} to the repository {request.TargetNugetRepository}");

                DotNetTasks.DotNetNuGetPush(publishSettings);

                _nukeLogger.Success($"Success Pushing Project {nuget.fileName}");
            }

            if (nuggets.Any())
            {
                var msg = $"The next nuget packages have been pushed:\n{string.Join("\n", nuggets.Select(x => x.fileName).ToArray())}";
                _nukeLogger.Success(msg);

                if (request.CommentToPrRequest != null)
                {
                    _nukeLogger.Info("Listing nuget packages pushed in the PR");
                    var githubToken = EnvironmentInfo.GetVariable<string>(EnvVar.GITHUB_TOKEN.ToString());

                    var client = new GitHubClient(new ProductHeaderValue(request.Originator));
                    client.Credentials = new Credentials(githubToken);

                    var issue = await client.Issue.Get(request.CommentToPrRequest.RepositoryId, request.CommentToPrRequest.PullRequestNumber);
                    var commentsClient = client.Issue.Comment;
                    await commentsClient.Create(request.CommentToPrRequest.RepositoryId, request.CommentToPrRequest.PullRequestNumber, msg);
                }
            }
            else
                _nukeLogger.Success("No nuget packages found");

            return Result.Ok();
        }
    }
}