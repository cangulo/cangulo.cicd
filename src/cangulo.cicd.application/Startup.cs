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
                .AddDomainServices(context);

            return services.BuildServiceProvider(true);
        }
    }
}
