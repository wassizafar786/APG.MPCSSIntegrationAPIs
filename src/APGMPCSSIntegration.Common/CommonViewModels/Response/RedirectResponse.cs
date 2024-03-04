using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APGMPCSSIntegration.Common.CommonViewModels.Response
{
    public class RedirectResponse<T>
    {
        public int Code { get; private set; }

        public bool Success { get; private set; }

        public string Description { get; private set; }

        public T Result { get; set; } // Contains the result of the function based on their type.
        public bool isSignatureVerified { get; set; }
        public RedirectResponse(bool success, bool isVerified, T result, int code, string description)
        {
            Result = result;
            isSignatureVerified = isVerified;
            Success = success;
            Code = code;
            Description = description;
        }

    }
}
