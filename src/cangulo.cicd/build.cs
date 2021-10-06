using cangulo.cicd;
using cangulo.cicd.abstractions.Constants;
using cangulo.cicd.abstractions.Models.CICDFile;
using cangulo.cicd.domain.Extensions;
using Nuke.Common;
using System;
using System.IO;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.UpdateVersionInCICDFile);

    private readonly IServiceProvider _serviceProvider;

    public Build()
    {
        SetupCICDSettings();

        var domainServiceContext = new DomainServiceContext
        {
            ResultBagFilePath = ResultBagFilePath
        };

        // TODO: avoid this changelog settings here
        var changelogSettings = CICDFile.ChangelogSettings;

        _serviceProvider = Startup.RegisterServices(domainServiceContext, changelogSettings);
    }

    private void SetupCICDSettings()
    {
        if (File.Exists(CICDFilePath))
        {
            var cicdFileContent = File.ReadAllText(CICDFilePath);
            CICDFile = JsonSerializer.Deserialize<CICDFileModel>(cicdFileContent, SerializerContants.DESERIALIZER_OPTIONS);

            Logger.Trace($"Request Mapped {JsonSerializer.Serialize(CICDFile, SerializerContants.SERIALIZER_OPTIONS)}");
        }
        else
            throw new Exception("cicd.json not provided in the root directory");
    }
}