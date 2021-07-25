using cangulo.cicd.domain.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace cangulo.cicd.domain.Extensions
{
    public static class DomainServiceCollectionExtension
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            return services
                .AddTransient<ICommitParser, CommitParser>();
        }
    }
}
