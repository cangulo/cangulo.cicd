using cangulo.build.Abstractions.Models;
using cangulo.build.Application.Requests;
using FluentResults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace cangulo.build.Application.RequestHandlers
{
    public class ExecuteAllUnitTestsInTheRepositoryRequestHandler : ICLIRequestHandler<ExecuteAllUnitTestsInTheRepository>
    {
        private readonly BuildContext _buildContext;
        private readonly ICLIRequestHandler<ExecuteUnitTests> _executeUTHandler;

        public ExecuteAllUnitTestsInTheRepositoryRequestHandler(BuildContext buildContext, ICLIRequestHandler<ExecuteUnitTests> executeUTHandler)
        {
            _buildContext = buildContext ?? throw new ArgumentNullException(nameof(buildContext));
            _executeUTHandler = executeUTHandler ?? throw new ArgumentNullException(nameof(executeUTHandler));
        }

        public Task<Result> Handle(ExecuteAllUnitTestsInTheRepository request, CancellationToken cancellationToken)
            => _executeUTHandler.Handle(
                    new ExecuteUnitTests
                    {
                        Solutions = _buildContext.Solutions.Select(x => x.Name).ToArray()
                    }, cancellationToken);
    }
}