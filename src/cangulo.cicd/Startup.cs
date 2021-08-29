using cangulo.changelog.Extensions;
using cangulo.changelog.Models;
using cangulo.cicd.domain.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace cangulo.cicd
{
    public static class Startup
    {
        public static ServiceProvider RegisterServices(DomainServiceContext context, ChangelogSettings changelogSettings)
        {
            var services = new ServiceCollection();

            services
                .AddDomainServices(context);

            if (changelogSettings is not null)
            {
                services
                    .AddChangelogServices(changelogSettings);
            }

            return services.BuildServiceProvider(true);
        }
    }

}
