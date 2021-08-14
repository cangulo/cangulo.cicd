using cangulo.cicd.domain.Helpers;
using cangulo.cicd.domain.Parsers;
using cangulo.cicd.domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cangulo.cicd.domain.Extensions
{
    public static class DomainServiceCollectionExtension
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ICommitParser, CommitParser>()
                .AddTransient<IReleaseNumberParser, ReleaseNumberParser>()
                .AddTransient<IPullRequestService, PullRequestService>()
                .AddTransient<IChangeLogService, ChangeLogService>()
                .AddTransient<INextReleaseNumberHelper, NextReleaseNumberHelper>();
        }
    }
}
