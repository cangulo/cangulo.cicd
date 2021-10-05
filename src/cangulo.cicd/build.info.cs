using cangulo.cicd.abstractions.Constants;
using Nuke.Common;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target PrintPipelineInfo => _ => _
        .Executes(() =>
        {
            Logger.Success($"GitRepo");
            Logger.Info(JsonSerializer.Serialize(GitRepository, SerializerContants.SERIALIZER_OPTIONS));

            Logger.Success($"GitHubAction");
            Logger.Info(JsonSerializer.Serialize(GitHubActions, SerializerContants.SERIALIZER_OPTIONS));

            Logger.Success($"Environment Variables:");

            EnvironmentInfo.Variables
                .ToList()
                .ForEach(x => Logger.Info($"{x.Key} : {x.Value}"));
        });
}