using cangulo.build.Abstractions.Models;
using cangulo.build.Application.Requests;
using cangulo.build.Application.Requests.Enums;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class PackProjectRequestValidator : AbstractValidator<PackProjects>
    {
        public PackProjectRequestValidator(BuildContext buildContext)
        {
            RuleFor(x => x.CreationMode)
                .NotNull()
                .NotEqual(NugetPackModeEnum.Undefined);
            RuleFor(x => x.Projects)
                .NotNull()
                .NotEmpty()
                .ForEach(x => x.ValidateProjectName(buildContext.Projects));
            RuleFor(x => x.OutputFolder)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.Branch)
                .NotNull()
                .NotEmpty();
        }
    }
}