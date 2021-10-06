using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System.Linq;
using cangulo.changelog.builders;
using cangulo.cicd.domain.Repositories;

internal partial class Build : NukeBuild
{
    private Target UpdateChangelog => _ => _
        .DependsOn(CalculateNextReleaseNumber)
        .Before(GitPushReleaseFiles)
        .Executes(() =>
        {
            var resultBagRepository = _serviceProvider.GetRequiredService<IResultBagRepository>();
            var changelogBuilder = _serviceProvider.GetRequiredService<IChangelogBuilder>();

            var nextReleaseNumber = resultBagRepository.GetResult(nameof(CalculateNextReleaseNumber));
            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommitsInThisPR));

            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found in the resultbag. Please execute the target {nameof(ListCommitsInThisPR)} before");

            changelogBuilder.Build(nextReleaseNumber, commitMsgs, ChangelogPath);

            Logger.Success($"updated changelog file {ChangelogPath}");
        });
}