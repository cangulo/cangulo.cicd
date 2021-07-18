using cangulo.build.Abstractions.Models;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class PackAllProjectsInTheRepositoryValidator : AbstractValidator<PackAllProjectsInTheRepository>
    {
        public PackAllProjectsInTheRepositoryValidator(BuildContext buildContext)
        {
            RuleFor(x => x.CreationMode)
                .NotNull()
                .NotEqual(NugetPackModeEnum.Undefined);
            RuleFor(x => x.OutputFolder)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.Branch)
                .NotNull()
                .NotEmpty();
        }
    }
}