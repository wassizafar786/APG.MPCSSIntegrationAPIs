using APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces;
using APGDigitalIntegration.DomainHelper;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Validators
{
    public class MerchantRefIdByMerchantIdResponseValidator : AbstractResponseValidator<long>, IMerchantRefIdByMerchantIdResponseValidator
    {
        public MerchantRefIdByMerchantIdResponseValidator(IStringLocalizer<SharedResource> sharedLocalizer) 
            : base(sharedLocalizer)
        {
        }

        public MerchantRefIdByMerchantIdResponseValidator AddValidTerminalRefIdValidator()
        {
            RuleFor(id => id.Data)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("Invalid Ref Id");

            return this;
        }
    }
}