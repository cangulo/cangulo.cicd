using cangulo.build.abstractions.Models;
using cangulo.build.abstractions.Models.Enums;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using cangulo.build.domain;
using FluentResults;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.GitVersion;
using Octokit;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.build.Application.RequestHandlers
{
    public class ProcessCommitActionsRequestHandler : ICLIRequestHandler<ProcessCommitActions, ProgramVersion>
    {
        private readonly ITagsService _tagsService;
        private readonly IVersionService _versionService;
        private readonly INukeLogger _nukeLogger;
        private readonly GitRepository _gitRepository;

        public ProcessCommitActionsRequestHandler(ITagsService tagsService, IVersionService versionService, INukeLogger nukeLogger, GitRepository gitRepository)
        {
            _tagsService = tagsService ?? throw new ArgumentNullException(nameof(tagsService));
            _versionService = versionService ?? throw new ArgumentNullException(nameof(versionService));
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
            _gitRepository = gitRepository ?? throw new ArgumentNullException(nameof(gitRepository));
        }

        public async Task<Result<ProgramVersion>> Handle(ProcessCommitActions request, CancellationToken cancellationToken)
        {
            var githubToken = EnvironmentInfo.GetVariable<string>(EnvVar.GITHUB_TOKEN.ToString());

            var client = new GitHubClient(new ProductHeaderValue(request.Originator));
            client.Credentials = new Credentials(githubToken);

            var actions = request.CommitActions.Where(x => x != CommitAction.Undefined);
            if (actions.Any())
            {
                // CreateMajor,CreateMinor,CreatePath workflow
                if (request.CommitActions.Any(x => x.ToString().ToLowerInvariant().Contains("create")))
                {
                    var tagsFromGitRepository = _gitRepository.Tags;
                    var info = new
                    {
                        _gitRepository.Branch,
                        _gitRepository.Commit,
                        _gitRepository.RemoteBranch,
                        _gitRepository.RemoteName,
                        _gitRepository.Tags
                    };
                    _nukeLogger.Info($"repo info{JsonSerializer.Serialize(info)}");


                    tagsFromGitRepository.ToList().ForEach(x => _nukeLogger.Info($"tag: {x}"));

                    var lastTag2 = _tagsService.GetLastTag();
                    _nukeLogger.Info($"lastTag: {lastTag2}");

                    if (string.IsNullOrEmpty(lastTag2))
                        return Result.Fail("No last tag defined in the project");

                    var currentProgramVersion = _versionService.ParseVersionFromTag(lastTag2);
                    var nextProgramVersion = new ProgramVersion();

                    _nukeLogger.Info($"current program version: {JsonSerializer.Serialize(currentProgramVersion)}");

                    if (request.CommitActions.Any(x => x == CommitAction.CreateMajor))
                    {
                        nextProgramVersion = new ProgramVersion { Major = currentProgramVersion.Major + 1, Minor = 0, Patch = 0 };
                    }
                    else if (request.CommitActions.Any(x => x == CommitAction.CreateMinor))
                    {
                        nextProgramVersion = new ProgramVersion { Major = currentProgramVersion.Major, Minor = currentProgramVersion.Minor + 1, Patch = 0 };
                    }
                    else if (request.CommitActions.Any(x => x == CommitAction.CreatePatch))
                    {
                        nextProgramVersion = new ProgramVersion { Major = currentProgramVersion.Major, Minor = currentProgramVersion.Minor, Patch = currentProgramVersion.Patch + 1 };
                    }
                    _nukeLogger.Info($"increased program version: {JsonSerializer.Serialize(nextProgramVersion)}");

                    return Result.Ok(nextProgramVersion);
                }
            }
            else
                _nukeLogger.Warn("No Action provided");

            return Result.Ok<ProgramVersion>(null);
        }
    }
}