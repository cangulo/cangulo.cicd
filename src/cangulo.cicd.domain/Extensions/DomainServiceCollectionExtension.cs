using cangulo.cicd.domain.Helpers;
using cangulo.cicd.domain.Parsers;
using cangulo.cicd.domain.Repositories;
using cangulo.cicd.domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common.IO;

namespace cangulo.cicd.domain.Extensions
{
    public class DomainServiceContext
    {
        public AbsolutePath ResultBagFilePath { get; set; }
    }
    public static class DomainServiceCollectionExtension
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services, DomainServiceContext context)
        {
            return services
                .AddTransient<ICommitParser, CommitParser>()
                .AddTransient<IReleaseNumberParser, ReleaseNumberParser>()
                .AddTransient<IPullRequestService, PullRequestService>()
                .AddTransient<IResultBagRepository, ResultBagRepository>(s => new ResultBagRepository(context.ResultBagFilePath))
                .AddTransient<INextReleaseNumberHelper, NextReleaseNumberHelper>();
        }
    }
}
