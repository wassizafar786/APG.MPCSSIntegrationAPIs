using APGDigitalIntegration.DomainHelper;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Validators;

public class CheckMerchantOrderNumberOfPaymentsValidator : AbstractResponseValidator<bool?>, ICheckMerchantOrderNumberOfPaymentsValidator
{
    public CheckMerchantOrderNumberOfPaymentsValidator(IStringLocalizer<SharedResource> sharedLocalizer) : base(sharedLocalizer)
    {
    }

    public CheckMerchantOrderNumberOfPaymentsValidator AddCheckLimitSuccessValidation()
    {
        RuleFor(t => t.Data)
            .Equal(true);

        return this;
    }

    public CheckMerchantOrderNumberOfPaymentsValidator GetValidator()
    {
        return new CheckMerchantOrderNumberOfPaymentsValidator(SharedLocalizer)
            .AddCheckLimitSuccessValidation();
    }
}