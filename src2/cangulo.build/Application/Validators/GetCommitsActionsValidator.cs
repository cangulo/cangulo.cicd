using cangulo.build.Application.Requests;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class GetCommitsActionsValidator : AbstractValidator<GetCommitsActions>
    {
        public GetCommitsActionsValidator()
        {
            RuleFor(x => x.PullRequestNumber)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.RepositoryId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => GetCommitsActions.EnvVarsRequired)
                .NotNull()
                .ForEach(x => x.ValidateEnvVarIsProvided());
        }
    }
}