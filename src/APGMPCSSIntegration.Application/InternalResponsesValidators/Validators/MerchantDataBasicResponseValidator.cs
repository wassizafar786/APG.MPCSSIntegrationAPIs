using System.Collections.Generic;
using System.Data;
using APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces;
using APGDigitalIntegration.DomainHelper;
using APGDigitalIntegration.IAL.Internal.Viewmodel.APGFundamentals.MerchantData;
using APGMPCSSIntegration.Constant;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Validators
{
    public class MerchantDataBasicResponseValidator : AbstractResponseValidator<List<MerchantDataBasicViewModel>>,
        IMerchantDataBasicResponseValidator
    {
        public MerchantDataBasicResponseValidator(IStringLocalizer<SharedResource> sharedLocalizer)
            : base(sharedLocalizer)
        {
        }

        public MerchantDataBasicResponseValidator AddCountValidation()
        {
            RuleFor(t => t.Data)
                .NotEmpty()
                .WithMessage(SharedLocalizer[ResponseMessages.NoMerchantsFoundForThisTerminal]);

            return this;
        }
        
        public MerchantDataBasicResponseValidator GetInstance() => this;
    }
}