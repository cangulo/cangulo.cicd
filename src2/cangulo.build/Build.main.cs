using cangulo.build.Abstractions.Models;
using cangulo.build.Application.Requests.Enums;
using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static cangulo.build.Models.ApplicationLayerConstants;

namespace cangulo.Build
{
    public partial class Build : NukeBuild
    {
        public static int Main() => Execute<Build>(x => x.GenericRequestHandler);

        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;


        public Build()
        {
            var solutions = RootDirectory
                .GlobFiles("**/*.sln")
                .Select(x => ProjectModelTasks.ParseSolution(x));

            if (solutions.Any())
                Logger.Info($"Found the next solutions:\n\t{string.Join("\n\t", solutions.Select(x => x.Name).ToArray())}");
            else
                Logger.Info($"No solution Found");

            var buildContext = new BuildContext
            {
                RootDirectory = RootDirectory,
                Solutions = solutions,
                Projects = solutions.SelectMany(x => x.AllProjects).Distinct()
            };

            _serviceProvider = Startup.RegisterServices(buildContext);
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
        }

        Target GenericRequestHandler => _ => _
            .Requires(() => ValidateRequest())
            .Executes(() => HandleRequest());

        private async Task HandleRequest()
        {
            var resultDynamic = await _mediator.Send(Request);
            var requestType = Request.GetType();

            if (resultDynamic is Result)
            {
                var result = resultDynamic as Result;

                ControlFlow.Assert(result.IsSuccess, $"Errors handling the request {Request.RequestModel}.Body:" +
                        $"\n{RequestJSON}" +
                        $"\nFound the next errors:" +
                        $"\n{string.Join("\t", result.Errors.Select(x => x.Message).ToArray())}");

                Logger.Info($"Success handling {requestType.Name}");
            }
            else if (resultDynamic is ResultBase)
            {
                var result = resultDynamic as ResultBase;
                ControlFlow.Assert(result.IsSuccess, $"Errors handling the request {Request.RequestModel}.Body:" +
                        $"\n{RequestJSON}" +
                        $"\nFound the next errors:" +
                        $"\n\r\t{string.Join("\n\r\t", result.Errors.Select(x => x.ToString()).ToArray())}");

                var output = result.GetType().GetProperty("Value").GetValue(result, null) ?? throw new ArgumentNullException($"result after handling request is null"); ;
                var outputType = output.GetType();

                var outputFolder = EnvironmentInfo.GetVariable<string>(EnvVar.OUTPUT_FILE_PATH.ToString());
                if (string.IsNullOrEmpty(outputFolder))
                    throw new ArgumentNullException($"no output folder provided");

                var outputFilename = $"{Request.RequestModel}_RESPONSE.json";
                var outputFolderAbsolutePath = RootDirectory / outputFolder;
                FileSystemTasks.EnsureExistingDirectory(outputFolderAbsolutePath);
                var fileAbsolutePath = outputFolderAbsolutePath / outputFilename;

                using FileStream createStream = File.Create(fileAbsolutePath);
                await JsonSerializer.SerializeAsync(createStream, output, outputType, SerializerContants.SERIALIZER_OPTIONS);

                Logger.Info($"Success handling {requestType.Name} output saved in the file {outputFilename} at {outputFolder}");
            };
        }

        private Target TestReadVariables => _ => _
             .Executes(() =>
             {
                 EnvironmentInfo.Variables.ToList().ForEach(x => Logger.Info($"{x.Key}:{x.Value}"));
             });
    }
}