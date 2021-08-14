using cangulo.cicd.domain.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace cangulo.cicd
{
    public static class Startup
    {
        public static ServiceProvider RegisterServices()
        {
            var services = new ServiceCollection();

            services
                .AddDomainServices();

            return services.BuildServiceProvider(true);
        }
    }
}
