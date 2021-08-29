using cangulo.changelog.Models;
using cangulo.changelog.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace cangulo.changelog.IntegrationTests
{
    public static class ServicesForTestBuilder
    {
        public static ServiceProvider GetServiceProvider(ChangelogSettings changelogSettings)
        {
            var services = new ServiceCollection();
            return services
                .AddChangelogServices(changelogSettings)
                .BuildServiceProvider();
        }
    }
}
