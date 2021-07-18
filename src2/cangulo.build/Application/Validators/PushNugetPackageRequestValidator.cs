using cangulo.build.Application.Requests;
using FluentValidation;

namespace cangulo.build.Application.Validators
{
    public class PushNugetPackageRequestValidator : AbstractValidator<PushNugetPackages>
    {
        public PushNugetPackageRequestValidator()
        {
            RuleFor(x => x.NugetPackagesLocation)
                .NotNull()
                .NotEmpty()
                .ForEach(x => x.NotEmpty());
            RuleFor(x => x.TargetNugetRepository)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.CommentToPrRequest)
                .SetValidator(new AddCommentsToPRValidator())
                .When(x => x.CommentToPrRequest != null);
        }
    }
}