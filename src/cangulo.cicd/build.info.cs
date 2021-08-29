using cangulo.cicd.abstractions.Constants;
using Nuke.Common;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target PrintPipelineInfo => _ => _
        .Executes(() =>
        {
            Logger.Info($"GitRepo:\n{JsonSerializer.Serialize(GitRepository, SerializerContants.SERIALIZER_OPTIONS)}");
            Logger.Info($"GitHubAction:\n{JsonSerializer.Serialize(GitHubActions, SerializerContants.SERIALIZER_OPTIONS)}");

            Logger.Info($"Environment Variables");


            EnvironmentInfo.Variables
                .ToList()
                .ForEach(x => Logger.Info($"{x.Key} : {x.Value}"));
        });
}