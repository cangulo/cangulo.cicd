using Nuke.Common.Tools.DotNet;
using Nuke.Common;
using Nuke.Common.IO;
using System.Linq;
using System.IO;

internal partial class Build : NukeBuild
{
    private Target CreateReleaseNugetPackage => _ => _
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.NugetSettings);

            var request = CICDFile.NugetSettings.PackNugetSettings;
            var projectPath = RootDirectory / request.ProjectPath;
            var outputDirectory = RootDirectory / request.OutputDirectory;

            Logger.Info($"creating nuget package for the project {request.ProjectPath}");

            FileSystemTasks.EnsureExistingDirectory(outputDirectory);

            var packSettings = new DotNetPackSettings()
                                    .SetProject(projectPath)
                                    .SetOutputDirectory(outputDirectory)
                                    .SetIncludeSymbols(true)
                                    .SetSymbolPackageFormat(DotNetSymbolPackageFormat.snupkg);

            DotNetTasks.DotNetPack(packSettings);
        });
    private Target PushNugetPackages => _ => _
        .Executes(() =>
        {
            ControlFlow.NotNull(CICDFile.NugetSettings);

            var request = CICDFile.NugetSettings.PushNugetsSettings;
            var nugetDirectories = RootDirectory / request.NugetsDirectory;

            FileSystemTasks.EnsureExistingDirectory(nugetDirectories);



            var files = nugetDirectories
                            .GlobFiles("**/*.nupkg")
                            .Select(x =>
                                new
                                {
                                    path = x,
                                    fileName = Path.GetFileName(x)
                                });
            if (!files.Any())
            {
                Logger.Info($"no nuget packages found in the folder {nugetDirectories}");
                return;
            }

            files
                .ToList()
                .ForEach(x =>
                {
                    Logger.Info($"pushing {x.fileName}");

                    var pushSettings = new DotNetNuGetPushSettings()
                                            .SetTargetPath(x.path)
                                            .SetSource(request.Source);

                    if (request.ApiKeyRequired)
                        pushSettings.SetApiKey(NugetApiKey);

                    DotNetTasks.DotNetNuGetPush(pushSettings);
                });
        });
}