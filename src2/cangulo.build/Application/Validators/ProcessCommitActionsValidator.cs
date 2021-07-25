using cangulo.build.Application.Requests;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class ProcessCommitActionsValidator : AbstractValidator<ProcessCommitActions>
    {
        public ProcessCommitActionsValidator()
        {
            RuleFor(x => GetCommitsActions.EnvVarsRequired)
                .NotNull()
                .ForEach(x => x.ValidateEnvVarIsProvided());
        }
    }
}