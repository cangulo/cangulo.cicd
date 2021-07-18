using FluentValidation;
using Nuke.Common.ProjectModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cangulo.build.Application.Validators.Shared
{
    public static class SolutionNameValidator
    {
        public static IRuleBuilderOptions<T, string>
           ValidateSolutionName<T>(this IRuleBuilder<T, string> ruleBuilder, IEnumerable<Solution> solutions)
        {
            return ruleBuilder
                    .Must(project => solutions
                                        .Where(x => StringComparer.OrdinalIgnoreCase.Equals(x.Name, project))
                                        .Count() == 1)
                    .WithMessage("The solution provided does not exists. Make sure the a .sln file exists with the name provided");
        }
    }
}