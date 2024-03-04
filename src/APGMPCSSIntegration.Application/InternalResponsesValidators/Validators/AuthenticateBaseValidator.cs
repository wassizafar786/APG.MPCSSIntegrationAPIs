
using APGDigitalIntegration.DomainHelper;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using FluentValidation;

using System.Threading;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Application.InternalResponsesValidators.Validators;
public class AuthenticateBaseValidator : AbstractValidator<AuthenticateBase>, IAuthenticateBaseValidator
{
    public AuthenticateBaseValidator AddPasswordValidation(int requestSourceId)
    {
        RuleFor(m => m.Password)
            .NotEmpty()
            .When(m => requestSourceId==(int)RequestSources.Portal||requestSourceId==(int)RequestSources.MerchantApp);
        return this;
    }

    public AuthenticateBaseValidator GetValidator(int requestSourceId)
    {
        return new AuthenticateBaseValidator()
                   .AddPasswordValidation(requestSourceId);
    }

    public virtual async Task ValidateAndThrowExceptionIfInvalid(AuthenticateBase context)
    {
        var validationResult = await this.ValidateAsync(context, CancellationToken.None).ConfigureAwait(false);
        if (validationResult.IsValid == false)
            throw new BusinessException(validationResult.GetErrorMessages(), validationResult.GetFirstErrorCode());
    }
}
