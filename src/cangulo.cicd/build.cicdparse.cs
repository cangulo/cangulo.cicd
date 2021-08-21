using Nuke.Common;
using System.IO;
using System;
using System.Text.Json;
using cangulo.cicd.abstractions.Models.CICDFile;
using cangulo.cicd.abstractions.Constants;

internal partial class Build : NukeBuild
{
    private Target ParseCICDFile => _ => _
        .Executes(async () =>
        {
            if (File.Exists(CICDFilePath))
            {
                using var openStream = File.OpenRead(CICDFilePath);
                CICDFile = await JsonSerializer.DeserializeAsync<CICDFileModel>(openStream, SerializerContants.DESERIALIZER_OPTIONS);

                Logger.Info($"Request Mapped {JsonSerializer.Serialize(CICDFile, SerializerContants.SERIALIZER_OPTIONS)}");
            }
            else
                throw new Exception("cicd.json not provided in the root directory");
        });
}