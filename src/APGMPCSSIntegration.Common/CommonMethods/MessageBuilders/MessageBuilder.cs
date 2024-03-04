using System.Xml;
using APGDigitalIntegration.Common.CommonViewModels.Common;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.DomainHelper.Services;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.DomainHelper.Services;
using Microsoft.Extensions.Options;

namespace APGDigitalIntegration.Common.CommonMethods.MessageBuilders
{
    public static class MessageBuilder
    {
        public static MqMessage ConstructMqMessage(string xmlRequest, string messageId, string messageType, string date)
        {
            try
            {
                if (xmlRequest != null && xmlRequest.Contains("<Document>"))
                {
                    if (messageType is MPCSSMessageTypes.CREDIT_MESSAGE_TYPE or MPCSSMessageTypes.DEBIT_MESSAGE_TYPE or MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE or MPCSSMessageTypes.PAYMENT_RETURN_MESSAGE_TYPE or MPCSSMessageTypes.PAYMENT_STATUS_REPORT_MESSAGE_TYPE)
                        xmlRequest = xmlRequest.Replace("<Document>", "<Document xmlns =\"urn:iso:std:iso:20022:tech:xsd:" + messageType + "\" xmlns:ns2 =\"http://www.Progressoft.com/ACH\">");
                    else
                        xmlRequest = xmlRequest.Replace("<Document>", "<Document xmlns =\"urn:ats:mpc:" + messageType + "\">");
                }
                var message = new MqMessage(xmlRequest, messageId, messageType);
                var obj = (object)xmlRequest.Replace("\r\n", "").Replace("\r", "");
                message.Signature = MpcssMethods.Sign((string)obj, date);
                var attributes = MpcssMethods.PrepareEnvelopeAttributes(message);
                message = MpcssMethods.WrapMessage(message, date, attributes);
                return message;
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }

}
