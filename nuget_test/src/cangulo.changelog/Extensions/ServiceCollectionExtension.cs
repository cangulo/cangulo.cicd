using cangulo.changelog.Models;
using cangulo.changelog.builders;
using Microsoft.Extensions.DependencyInjection;
using cangulo.changelog.Parsers;
using cangulo.changelog.Builders.ConventionalCommits;
using cangulo.changelog.Builders.NonConventionalCommits;
using cangulo.changelog.Builders;

namespace cangulo.changelog.Extensions
{
    public static class ChangelogServiceCollectionExtension
    {
        public static IServiceCollection AddChangelogServices(this IServiceCollection services, ChangelogSettings changelogSettings)
        {
            return services
                .AddTransient<IReleaseNotesBuilder, ReleaseNotesBuilder>()
                .AddTransient<IChangelogBuilder, ChangelogBuilder>()
                .AddDomainServices(changelogSettings);
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services, ChangelogSettings changelogSettings)
        {

            if (changelogSettings.CommitsMode is CommitsMode.ConventionalCommits)
                services
                    .AddTransient<IChangesListAreaBuilder, ChangesAreaBuilderForConventionalCommits>()
                    .AddTransient<IConventionalCommitParser, ConventionalCommitParser>()
                    .AddSingleton(changelogSettings);
            else if (changelogSettings.CommitsMode is CommitsMode.NonConventionalCommits)
                services
                    .AddTransient<IChangesListAreaBuilder, ChangesAreaBuilderForNonConventionalCommits>();

            services
                .AddTransient<IChangelogVersionNotesBuilder, ChangelogVersionNotesBuilder>();

            return services;
        }
    }
}