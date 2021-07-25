using cangulo.build.Application.Requests.Enums;
using FluentValidation;
using Nuke.Common;

namespace cangulo.build.Application.Validators.Shared
{
    public static class EnvVarProvidedValidator
    {
        public static IRuleBuilderOptions<T, EnvVar>
           ValidateEnvVarIsProvided<T>(this IRuleBuilder<T, EnvVar> ruleBuilder)
        {
            return ruleBuilder
                    .Must(envVar =>
                    {
                        var envVarValue = EnvironmentInfo.GetVariable<string>(envVar.ToString());
                        return !string.IsNullOrEmpty(envVarValue);
                    })
                    .WithMessage((x, envVar) => $"The Environment Variable {envVar} was not provided.");
        }
    }
}