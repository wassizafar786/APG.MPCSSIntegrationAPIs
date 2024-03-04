using System;
using System.Collections.Generic;
using System.Linq;

namespace APGMPCSSIntegration.DomainHelper.Exceptions
{
    public class BusinessException : Exception
    {
        #region Fields
        public List<string> ErrorList { get;}
        public string ResponseCode { get; set; }

        #endregion

        #region Constructors

        public BusinessException() { }

        public BusinessException(string error, string responseCode)
        {
            ErrorList = new List<string> { error };
            ResponseCode = responseCode;
        }

        public BusinessException(IEnumerable<string> errorList, string responseCode)
        {
            ErrorList = errorList.ToList();
            ResponseCode = responseCode;
        }

        #endregion

    }
}