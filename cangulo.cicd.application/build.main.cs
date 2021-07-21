using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System.Linq;
using System.IO;
using System;
using System.Text.Json;
using cangulo.cicd.Abstractions.Requests;
using cangulo.cicd.Abstractions.Constants;
using Nuke.Common.ProjectModel;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
internal partial class Build : NukeBuild
{
    public BaseRequest BaseRequest;

    public static int Main() => Execute<Build>(x => x.Compile);

    //private Target BuildSolution => _ => _
    //    .Requires(() => ValidateRequest())
    //    .Executes(() =>
    //     {
    //         var toolInstalled = DotNet("tool list -g", logOutput: true)
    //            .Select(x => x.Text)
    //            .Any(x => x.Contains("amazon.lambda.tools"));

    //         if (!toolInstalled)
    //         {
    //             DotNetToolInstall(
    //                 new DotNetToolInstallSettings()
    //                 .SetPackageName("Amazon.Lambda.Tools")
    //                 .SetGlobal(true));
    //         }
    //     });

    //private Target Clean => _ => _
    // .Before(Restore)
    // .Executes(() =>
    // {
    //     RootDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
    //     EnsureCleanDirectory(ArtifactsDirectory);
    // });

    //private Target ValidateCICDFile => _ => _
    // .After(ParseCICDFile)
    // .Executes(() =>
    // {
    //     RootDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
    //     EnsureCleanDirectory(ArtifactsDirectory);
    // });

    private Target ParseRequest => _ => _
        .Executes(() =>
        {
            var runningTarget = RunningTargets;
            Logger.Info($"ParseRequest: runningTarget: {runningTarget.First().Name}");
        });

    private Target ValidateRequest => _ => _
        .Executes(() =>
        {
            var runningTarget = RunningTargets;
            Logger.Info($"ValidateRequest: runningTarget: {runningTarget.First().Name}");

            var cicdFilePath = RootDirectory / "cicd.json";
            if (File.Exists(cicdFilePath))
            {
                var cicdContent = File.ReadAllText(cicdFilePath);

                var requestType = Type.GetType($"cangulo.cicd.Abstractions.Requests.{runningTarget.First().Name}Request");
                if (requestType is null)
                {
                    throw new Exception($"request provided is not supported");
                }

                BaseRequest = JsonSerializer.Deserialize(cicdContent, requestType, SerializerContants.DESERIALIZER_OPTIONS) as BaseRequest;
                Logger.Trace($"Request Mapped {JsonSerializer.Serialize(cicdContent, SerializerContants.SERIALIZER_OPTIONS)}");


                //var baseRequest = JsonSerializer.Deserialize<BaseCLIRequest>(cicdContent, SerializerContants.DESERIALIZER_OPTIONS);

                //TODO: Add validation
                //var solutionPath = RootDirectory / baseRequest.SolutionPath;
                //if (File.Exists(solutionPath))
                //{
                //    Solution = ProjectModelTasks.ParseSolution(solutionPath);

                //}
                //else
                //    throw new Exception("invalid SolutionPath provided");
            }
            else
                throw new Exception("cicd.json not provided");
        });

    private Target Restore => _ => _
        .Inherit(ParseRequest, ValidateRequest)
        .Executes(() =>
         {
             var restoreRequest = BaseRequest as RestoreRequest;
             var solutionPath = RootDirectory / restoreRequest.SolutionPath;
             if (File.Exists(solutionPath))
             {
                 var solution = ProjectModelTasks.ParseSolution(solutionPath);
                 DotNetRestore(s => s
                     .SetProjectFile(solution));
             }
             else
                 throw new Exception("invalid SolutionPath provided");

         });

    private Target Compile => _ => _
        .DependsOn(Restore)
        .Inherit(ParseRequest, ValidateRequest)
        .Executes(() =>
        {
            var restoreRequest = BaseRequest as CompileRequest;
            var solutionPath = RootDirectory / restoreRequest.SolutionPath;
            if (File.Exists(solutionPath))
            {
                var solution = ProjectModelTasks.ParseSolution(solutionPath);
                DotNetBuild(s => s
                    .SetProjectFile(solution)
                    .EnableNoRestore());
            }
            else
                throw new Exception("invalid SolutionPath provided");
        });
}