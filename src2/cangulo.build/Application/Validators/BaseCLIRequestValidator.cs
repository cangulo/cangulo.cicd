using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class BaseCLIRequestValidator : AbstractValidator<BaseCLIRequest>
    {
        public BaseCLIRequestValidator()
        {
            RuleFor(x => x.RequestModel)
                .NotNull()
                .NotEqual(CLIRequestModelEnum.Undefined)
                .WithMessage("The request model provided is not defined");
            RuleFor(x => x.Originator)
                .NotNull()
                .NotEmpty()
                .Must(x => !x.Contains("/"))
                .WithMessage("The originator can't contain backslash /");
        }
    }
}