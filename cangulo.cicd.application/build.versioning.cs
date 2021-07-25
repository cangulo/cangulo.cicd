using cangulo.cicd.Abstractions.Constants;
using Nuke.Common;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private Target ProposeNextVersion => _ => _
        .DependsOn(ParseCICDFile)
        .Executes(() =>
         {
             Logger.Info($"GitRepoInfo:{JsonSerializer.Serialize(GitRepository, SerializerContants.SERIALIZER_OPTIONS)}");
         });
}