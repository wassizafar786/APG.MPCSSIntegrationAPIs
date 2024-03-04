using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.DomainHelper;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.Application.InternalResponsesValidators
{
    public abstract class AbstractResponseValidator<T> : AbstractValidator<BaseResponse<T>>
    {
        protected readonly IStringLocalizer<SharedResource> SharedLocalizer;

        public AbstractResponseValidator(IStringLocalizer<SharedResource> sharedLocalizer)
        {
            SharedLocalizer = sharedLocalizer;
        }
        
        protected override bool PreValidate(ValidationContext<BaseResponse<T>> context, ValidationResult result)
        {
            if (context.InstanceToValidate.Success)
                return base.PreValidate(context, result);
            
            if (context.InstanceToValidate.ErrorList.Any())
            {
                context.InstanceToValidate.ErrorList.ForEach(error => 
                {
                    var errorMessage = string.IsNullOrWhiteSpace(error)
                        ? SharedLocalizer[ResponseMessages.ResponseFailure]
                        : error;

                    result.Errors.Add(new ValidationFailure("Success", errorMessage, "")
                    {
                        CustomState = ResponseCodes.Failure
                    });
                });
            }
            else
            {
                result.Errors.Add(new ValidationFailure("Success", SharedLocalizer[ResponseMessages.ResponseFailure], "")
                {
                    CustomState = context.InstanceToValidate.ResponseCode
                });
            }
            
            return false;
        }
        
        public virtual async Task ValidateAndThrowExceptionIfInvalid(BaseResponse<T> context)
        {
            var validationResult = await this.ValidateAsync(context,CancellationToken.None).ConfigureAwait(false);
            if (validationResult.IsValid == false)
            {
                var responseCode = string.IsNullOrWhiteSpace(context.ResponseCode)
                    ? ResponseCodes.Failure
                    : context.ResponseCode;
                
                throw new BusinessException(validationResult.GetErrorMessages(), responseCode);
            }
        }
        
    }
}