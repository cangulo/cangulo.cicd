using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.RequestHandlers;
using cangulo.build.Application.Requests;
using cangulo.build.domain.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common.Git;
using Nuke.Common.Tooling;
using System;
using System.Collections.Generic;
using static cangulo.build.domain.Extensions.DomainServiceCollectionExtension;

namespace cangulo.Build
{
    public static class Startup
    {
        public static ServiceProvider RegisterServices(BuildContext buildContext)
        {
            var services = new ServiceCollection();

            services
                .AddSingleton<BuildContext>(buildContext)
                .AddSingleton<GitRepository>(GitRepository.FromLocalDirectory(buildContext.RootDirectory))
                .AddSingleton<INukeLogger, NukeLogger>();

            services.AddMediatR(typeof(Startup));
            services
                .AddTransient<ICLIRequestHandler<ExecuteUnitTests>, ExecuteUnitTestsRequestHandler>()
                .AddTransient<ICLIRequestHandler<PackProjects>, PackProjectRequestHandler>();

            RegisterApplitactionLayerValidators(services);

            var gitPath = ToolPathResolver.TryGetEnvironmentExecutable("GIT_EXE") ?? ToolPathResolver.GetPathExecutable("git");

            services
                .AddDomainServices(
                    new DomainServiceContext
                    {
                        Git = ToolResolver.GetLocalTool(gitPath)
                    });

            return services.BuildServiceProvider(true);
        }

        private static void RegisterApplitactionLayerValidators(ServiceCollection services) => services.Scan(s => s
                        .FromAssemblyOf<CLIRequest>()
                        .AddClasses(s =>
                            s.AssignableTo(typeof(AbstractValidator<>)))
                        .As((x) =>
                        {
                            var requestType = x.BaseType.GenericTypeArguments[0];
                            var validatorType = typeof(AbstractValidator<>);
                            return new List<Type>() { validatorType.MakeGenericType(new Type[] { requestType }) };
                        })
        );
    }
}