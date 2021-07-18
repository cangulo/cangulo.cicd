using cangulo.build.Abstractions.Models;
using cangulo.build.Abstractions.NukeLogger;
using cangulo.build.Application.Requests;
using FluentResults;
using Nuke.Common.Tools.DotNet;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.build.Application.RequestHandlers
{
    public class ExecuteUnitTestsRequestHandler : ICLIRequestHandler<ExecuteUnitTests>
    {
        private readonly BuildContext _buildContext;
        private readonly INukeLogger _nukeLogger;

        public ExecuteUnitTestsRequestHandler(BuildContext buildContext, INukeLogger nukeLogger)
        {
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
            _nukeLogger = nukeLogger ?? throw new ArgumentNullException(nameof(nukeLogger));
        }

        public async Task<Result> Handle(ExecuteUnitTests request, CancellationToken cancellationToken)
        {
            foreach (var solutionName in request.Solutions)
            {
                var solution = _buildContext
                .Solutions
                .Single(x =>
                    x.Name == solutionName);

                _nukeLogger.Info($"Retoring Projects in the solution {solution.Name}");

                DotNetTasks
                    .DotNetRestore(s => s.SetProjectFile(solution));

                _nukeLogger.Info($"Executing UTs in the solution {solution.Name}");

                // TODO: Filter projects by property, is test and is packable

                var testSettings = solution.AllProjects
                    .Select(x =>
                        new DotNetTestSettings()
                        .SetProjectFile(x.Path)
                        .EnableNoRestore());

                DotNetTasks.DotNetTest(CombinatorialConfigure => (testSettings));

                _nukeLogger.Success($"Success Executing UT for the solution {solutionName}");
            }

            return Result.Ok();
        }
    }
}