namespace APGDigitalIntegration.Application.InternalResponsesValidators.Validators;
public interface IAuthenticateBaseValidator
{
    AuthenticateBaseValidator GetValidator(int requestSourceId);
}
