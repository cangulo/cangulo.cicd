using cangulo.build.Application.Requests;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class GetNumberLastPRMergedValidator : AbstractValidator<GetNumberLastPRMerged>
    {
        public GetNumberLastPRMergedValidator()
        {
            RuleFor(x => x.RepositoryId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => GetModifiedProjectsInPR.EnvVarsRequired)
                .NotNull()
                .ForEach(x => x.ValidateEnvVarIsProvided());
            RuleFor(x => x.TargetBranch)
                .NotNull()
                .NotEmpty();
        }
    }
}