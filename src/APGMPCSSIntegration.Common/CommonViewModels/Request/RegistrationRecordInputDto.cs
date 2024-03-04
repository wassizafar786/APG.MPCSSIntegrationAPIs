using APGMPCSSIntegration.Common.CommonViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace APGMPCSSIntegration.Common.CommonViewModels.Request
{
    public class RegistrationRecordInputDto
    {
        /*Identification reference of the message at the PSP*/
        public MessageIdentifierInputDto MessageIdentifier { get; set; }

        /*Customer Information*/
        [Required]
        public string CustomerId { get; set; } /*Eg: Passport Number of customer*/
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerType { get; set; }
        [Required]
        public DateTime DobOrRegistrationDate { get; set; }

        [MaxLength(4, ErrorMessage = "Maximum field lenght is 4")]
        [Required]
        public string IdentificationTypeCode { get; set; }

        //[RegularExpression(@"^[A-Z]{2,2}+$", ErrorMessage = "Invalid Country Code")]
        [MaxLength(2, ErrorMessage = "Maximum field lenght is 2")]
        [Required]
        public string IdIssuingCountryCode { get; set; }


        /*Address Information*/
        public string POBox { get; set; }
        public string PostalCode { get; set; }
        public string StreetName { get; set; }
        public string BuildingNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string CityName { get; set; }
        public string TownName { get; set; }
        public string GovernorateName { get; set; }
        public string CountryCode { get; set; }
        public int MpcssMessageType { get; set; }
        public DateTime CreationDateTime { get; set; }
        public string AdditionalInfo { get; set; }

            
        public string BankId { get; set; }
        public string ParticipantId { get; set; }
        
    }
}
