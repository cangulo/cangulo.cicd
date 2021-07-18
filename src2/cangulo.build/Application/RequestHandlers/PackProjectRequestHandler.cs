using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using FluentResults;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static cangulo.build.abstractions.Constants;

namespace cangulo.build.Application.RequestHandlers
{
    public class PackProjectRequestHandler : ICLIRequestHandler<PackProjects>
    {
        private readonly BuildContext _buildContext;
        private readonly INukeLogger _nukeLogger;

        public PackProjectRequestHandler(BuildContext buildContext, INukeLogger nukeLogger)
        {
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
        }

        public async Task<Result> Handle(PackProjects request, CancellationToken cancellationToken)
        {
            var outputFolderAbsolutePath = _buildContext.RootDirectory / request.OutputFolder;
            FileSystemTasks.EnsureCleanDirectory(outputFolderAbsolutePath);

            foreach (var projectName in request.Projects)
            {
                var solution = _buildContext
                                    .Solutions
                                    .Single(x =>
                                        x.AllProjects.Any(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, projectName)));

                DotNetTasks
                    .DotNetRestore(s => s.SetProjectFile(solution));

                var project = solution.AllProjects.Single(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, projectName));
                var configuration = request.CreationMode == NugetPackModeEnum.Prerelease ? "Debug" : "Release";

                _nukeLogger.Info($"Building Project {projectName}");

                DotNetTasks
                    .DotNetBuild(s => s
                        .SetProjectFile(solution)
                        .SetConfiguration(configuration)
                        .EnableNoRestore());

                var currentPackageVersion = project.GetProperty<string>(CSProjProperties.VERSION_PREFIX) ?? throw new ArgumentNullException($"project {projectName} doesn't have a versionPrefix defined in the csproj: {project.Path}");
                string versionSuffix = BuildVersionSuffix(request, configuration);

                _nukeLogger.Success($"Success Building Project {projectName}");

                _nukeLogger.Info($"Packing Project {projectName}");

                DotNetTasks
                    .DotNetPack(s => s
                        .SetProject(project.Path)
                        .SetOutputDirectory(outputFolderAbsolutePath)
                        .SetConfiguration(configuration)
                        .SetVersionPrefix(currentPackageVersion)
                        .SetVersionSuffix(versionSuffix)
                        .EnableNoRestore());

                _nukeLogger.Success($"Success Packing Project {projectName}");
            }

            _nukeLogger.Success($"Nuget Packages located at {outputFolderAbsolutePath}");

            return Result.Ok();
        }

        private static string BuildVersionSuffix(PackProjects request, string configuration)
            => StringComparer.OrdinalIgnoreCase.Equals(configuration, "Debug") ?
                $"{request.Branch.Replace("/", "")}-{DateTime.Now:yyyyMMdd-HHmm}" :
                string.Empty;
    }
}