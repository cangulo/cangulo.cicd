using Nuke.Common;
using System.IO;
using System;
using System.Text.Json;
using cangulo.cicd.abstractons.Models.CICDFile;
using cangulo.cicd.Abstractions.Constants;

internal partial class Build : NukeBuild
{
    private Target ParseCICDFile => _ => _
        .Executes(() =>
        {
            var cicdFilePath = RootDirectory / "cicd.json";
            if (File.Exists(cicdFilePath))
            {
                var cicdContent = File.ReadAllText(cicdFilePath);

                CICDFile = JsonSerializer.Deserialize<CICDFileModel>(cicdContent, SerializerContants.DESERIALIZER_OPTIONS);

                Logger.Info($"Request Mapped {JsonSerializer.Serialize(CICDFile, SerializerContants.SERIALIZER_OPTIONS)}");
            }
            else
                throw new Exception("cicd.json not provided in the root directory");
        });
}