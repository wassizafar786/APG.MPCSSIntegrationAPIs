using APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces;
using APGDigitalIntegration.DomainHelper;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Validators
{
    public class CheckShadowBalanceLimitResponseValidator : AbstractResponseValidator<object>, ICheckShadowBalanceLimitResponseValidator
    {
        public CheckShadowBalanceLimitResponseValidator(IStringLocalizer<SharedResource> sharedLocalizer) : base(sharedLocalizer)
        {
        }

        public CheckShadowBalanceLimitResponseValidator AddCheckLimitSuccessValidation()
        {
            RuleFor(t => t.Data.ToString())
                .Equals("true");

            return this;
        }

        public CheckShadowBalanceLimitResponseValidator GetValidator()
        {
            return new CheckShadowBalanceLimitResponseValidator(SharedLocalizer)
                .AddCheckLimitSuccessValidation();
        }
    }
}