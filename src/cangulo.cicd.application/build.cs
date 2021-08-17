using cangulo.cicd;
using cangulo.cicd.domain.Extensions;
using Nuke.Common;
using System;

internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.UpdateVersionInFiles);

    private readonly IServiceProvider _serviceProvider;

    public Build()
    {
        _serviceProvider = Startup.RegisterServices(
            new DomainServiceContext
            {
                ResultBagFilePath = ResultBagFilePath
            });
    }
}