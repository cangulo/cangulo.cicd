using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using FluentResults;
using Nuke.Common;
using Nuke.Common.IO;
using Octokit;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.build.Application.RequestHandlers
{
    public class CreateReleaseRequestHandler : ICLIRequestHandler<CreateRelease>
    {
        private readonly INukeLogger _nukeLogger;
        private readonly BuildContext _buildContext;

        public CreateReleaseRequestHandler(INukeLogger nukeLogger, BuildContext buildContext)
        {
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
        }

        public async Task<Result> Handle(CreateRelease request, CancellationToken cancellationToken)
        {
            var githubToken = EnvironmentInfo.GetVariable<string>(EnvVar.GITHUB_TOKEN.ToString());

            var ghClient = new GitHubClient(new ProductHeaderValue(request.Originator));
            ghClient.Credentials = new Credentials(githubToken);
            var client = ghClient.Repository.Release;


            var newReleaseData = new NewRelease(request.Tag)
            {
                Name = request.Title
            };

            var releaseCreated = await client.Create(request.RepositoryId, newReleaseData);

            var releaseCreatedInfo = new
            {
                releaseCreated.Id,
                releaseCreated.Name,
                releaseCreated.TagName,
                releaseCreated.TargetCommitish,
                releaseCreated.AssetsUrl
            };
            _nukeLogger.Success($"Created release: {JsonSerializer.Serialize(releaseCreatedInfo)}");

            var assetsFiles = (_buildContext.RootDirectory / request.ReleaseAssetsFolder).GlobFiles("**/*");
            foreach (var file in assetsFiles)
            {
                var fileName = Path.GetFileName(file);
                _nukeLogger.Info($"uploading file: {fileName}");

                var assetData = new ReleaseAssetUpload
                {
                    FileName = fileName,
                    RawData = File.OpenRead(file),
                    ContentType = "application/zip"
                };
                await client.UploadAsset(releaseCreated, assetData, cancellationToken);
            }

            return Result.Ok();
        }
    }
}