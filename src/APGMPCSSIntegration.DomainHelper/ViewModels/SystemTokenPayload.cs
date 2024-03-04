using System;

namespace APGDigitalIntegration.DomainHelper.Models
{
    public class SystemTokenPayload
    {
        public string Id {get; set;}
        public string CreationDate {get; set;}
        public string ExpireyDate {get; set;}
        public string RequestSource { get; set; }


        public bool IsExpiryDateValid()
        {
            if (DateTime.TryParse(ExpireyDate, out DateTime expiryDateTime))
            {
                return expiryDateTime > DateTime.UtcNow;
            }

            return false;
        }
    }
}
