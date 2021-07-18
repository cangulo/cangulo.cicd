using cangulo.build.Application.Requests;
using FluentResults;
using MediatR;

namespace cangulo.build.Application.RequestHandlers
{
    public interface ICLIRequestHandler<in T> : IRequestHandler<T, Result> where T : IRequest<Result>
    {
    }

    public interface ICLIRequestHandler<in T, Z> : IRequestHandler<T, Result<Z>> where T : CLIRequest<Z>
    {
    }
}