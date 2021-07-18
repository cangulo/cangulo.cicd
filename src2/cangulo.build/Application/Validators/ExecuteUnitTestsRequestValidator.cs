using cangulo.build.Abstractions.Models;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class ExecuteUnitTestsRequestValidator : AbstractValidator<ExecuteUnitTests>
    {
        public ExecuteUnitTestsRequestValidator(BuildContext buildContext)
        {
            RuleFor(x => x.Solutions)
                .NotNull()
                .NotEmpty()
                .ForEach(x => x.NotNull().NotEmpty().ValidateSolutionName(buildContext.Solutions));
        }
    }
}