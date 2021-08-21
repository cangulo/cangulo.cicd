using cangulo.changelog.builders;
using cangulo.cicd.domain.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace cangulo.cicd
{
    public static class Startup
    {
        public static ServiceProvider RegisterServices(DomainServiceContext context)
        {
            var services = new ServiceCollection();

            services
                .AddApplicationServices()
                .AddDomainServices(context);

            return services.BuildServiceProvider(true);
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IReleaseNotesBuilder, ReleaseNotesBuilder>();
        }
    }

}
