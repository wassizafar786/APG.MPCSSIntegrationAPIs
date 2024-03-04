using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace APGDigitalIntegration.DomainHelper
{
    public static class ValidationResultExtensions
    {
        public static IEnumerable<string> GetErrorMessages(this ValidationResult validationResult)
        {
            return validationResult.Errors.Select(e => e.ErrorMessage);
        }        
        public static string GetFirstErrorCode(this ValidationResult validationResult)
        {
            return validationResult.Errors.Select(e => e.CustomState?.ToString()).FirstOrDefault();
        }
        
        public static string GetFirstErrorMessage(this ValidationResult validationResult)
        {
            return validationResult.Errors.Select(e => e.ErrorMessage).FirstOrDefault();
        }
        
        
    }
}