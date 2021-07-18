using cangulo.build.Application.Requests.Enums;
using FluentResults;
using MediatR;

namespace cangulo.build.Application.Requests
{
    public class CLIRequest : BaseCLIRequest, IRequest<Result> { }

    public class CLIRequest<T> : BaseCLIRequest, IRequest<Result<T>> { }

    public class BaseCLIRequest
    {
        public CLIRequestModelEnum RequestModel { get; set; }
        public string Originator { get; set; }
        public static EnvVar[] EnvVarsRequired { get; }
    }
}