using FluentValidation;
using Nuke.Common.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cangulo.build.Application.Validators.Shared
{
    public static class ProjectNameValidator
    {
        public static IRuleBuilderOptions<T, string>
           ValidateProjectName<T>(this IRuleBuilder<T, string> ruleBuilder, IEnumerable<Project> projects)
        {
            return ruleBuilder
                    .Must(project => projects
                                        .Any(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, project)))
                    .WithMessage("The project is not linked to any solution");
        }
    }
}