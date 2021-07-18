using Microsoft.Extensions.DependencyInjection;
using Nuke.Common.Tooling;

namespace cangulo.build.domain.Extensions
{
    public static class DomainServiceCollectionExtension
    {
        public class DomainServiceContext
        {
            public Tool Git { get; set; }
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services, DomainServiceContext serviceContext)
        {
            return services
                .AddTransient<ICommitMessageService, CommitMessageService>()
                .AddTransient<ITagsService, TagsService>((provider) => new TagsService(serviceContext.Git))
                .AddTransient<IVersionService, VersionService>();
            //.AddTransient<ICreateReleaseCIProcessor,CreatePatchProcessor>()
            //.AddTransient<ICreateReleaseCIProcessor,CreateMinorProcessor>()
            //.AddTransient<ICreateReleaseCIProcessor, CreateMajorProcessor>();
        }
    }
}