using cangulo.build.Application.Requests;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class GetModifiedProjectsInPRValidator : AbstractValidator<GetModifiedProjectsInPR>
    {
        public GetModifiedProjectsInPRValidator()
        {
            RuleFor(x => x.PullRequestNumber)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.RepositoryId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => GetModifiedProjectsInPR.EnvVarsRequired)
                .NotNull()
                .ForEach(x => x.ValidateEnvVarIsProvided());
        }
    }
}