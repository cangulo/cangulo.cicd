using cangulo.build.Application.Requests;
using cangulo.build.Application.Validators.Shared;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class CreateReleaseValidator : AbstractValidator<CreateRelease>
    {
        public CreateReleaseValidator()
        {
            RuleFor(x => x.RepositoryId)
                .NotNull()
                .GreaterThan(0);
            RuleFor(x => x.Tag)
                .NotEmpty();
            RuleFor(x => x.Title)
                .NotEmpty();
            RuleFor(x => x.ReleaseNotesFilePath)
                .NotEmpty();
            RuleFor(x => x.ReleaseAssetsFolder)
                .NotEmpty();
            RuleFor(x => CreateRelease.EnvVarsRequired)
                .NotNull()
                .ForEach(x => x.ValidateEnvVarIsProvided());
        }
    }
}