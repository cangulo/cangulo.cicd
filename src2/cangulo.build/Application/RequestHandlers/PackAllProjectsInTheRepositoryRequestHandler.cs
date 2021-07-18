using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Extensions;
using FluentResults;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static cangulo.build.abstractions.Constants;

namespace cangulo.build.Application.RequestHandlers
{
    public class PackAllProjectsInTheRepositoryRequestHandler : ICLIRequestHandler<PackAllProjectsInTheRepository>
    {
        private readonly ICLIRequestHandler<PackProjects> _packProjectHandler;
        private readonly BuildContext _buildContext;
        private readonly INukeLogger _nukeLogger;

        public PackAllProjectsInTheRepositoryRequestHandler(
            ICLIRequestHandler<PackProjects> packProjectHandler,
            BuildContext buildContext,
            INukeLogger nukeLogger)
        {
            _packProjectHandler = packProjectHandler ?? throw new ArgumentNullException(nameof(packProjectHandler));
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
        }

        public async Task<Result> Handle(PackAllProjectsInTheRepository request, CancellationToken cancellationToken)
        {
            var packableProjectNames = _buildContext
                                        .Solutions
                                        .SelectMany(x => PackableProjects(x))
                                        .Select(x => x.Name).ToArray();

            var executionResult = await _packProjectHandler.Handle(
                new PackProjects
                {
                    Branch = request.Branch,
                    CreationMode = request.CreationMode,
                    OutputFolder = request.OutputFolder,
                    Projects = packableProjectNames
                }, cancellationToken);

            var nugetPackagesCreated = (_buildContext.RootDirectory / request.OutputFolder).GlobFiles("**/*.nupkg").Select(x => x.GetFileName()).ToArray();
            _nukeLogger.Success($"All nuget packages created:\n{string.Join("\n", nugetPackagesCreated)}");

            return executionResult;
        }

        private static IEnumerable<Project> PackableProjects(Solution x)
            => x.AllProjects.Where(ProjectIsPackable());

        private static Func<Project, bool> ProjectIsPackable() => x =>
        {
            var result = true;
            var isPackableString = x.GetProperty(CSProjProperties.IS_PACKABLE);
            if (string.IsNullOrEmpty(isPackableString))
                return true;

            bool.TryParse(isPackableString, out result);
            return result;
        };
    }
}