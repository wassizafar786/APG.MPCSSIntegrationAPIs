using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.DomainHelper.Services;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using APGDigitalIntegration.Constant;


namespace APGMPCSSIntegration.Common.CommomMethods.MessageBuilders

{
    public class MpcssMethods
    {
        private static Security pspCertificate;
        private static Security mpcCertificate;

        public static string ConvertRequestToXMLString(object request)
        {
            try
            {
                string xmlRequest;
                XmlSerializer xmlSerializer = new XmlSerializer(request.GetType(), new XmlRootAttribute("Document"));
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    xmlSerializer.Serialize(memoryStream, request, ns);
                    memoryStream.Position = 0;
                    xmlRequest = new StreamReader(memoryStream).ReadToEnd();
                }

                if (xmlRequest.Length > 0)
                {
                    xmlRequest = xmlRequest.Replace("<?xml version=\"1.0\"?>", "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>");
                }

                XDocument xDoc = XDocument.Parse(xmlRequest, LoadOptions.None);
                xmlRequest = xDoc.ToString(SaveOptions.DisableFormatting);
                return xmlRequest;
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in ConvertRequestToXMLString: " + ex);
                return null;
            }

        }

        public static object ConvertXMLResponseToString<T>(string xmlResponse)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(xmlResponse);
                using var stream = new MemoryStream(buffer);
                var serializer = new XmlSerializer(typeof(T));
                var mpcssResponse = (T)serializer.Deserialize(stream);
                return mpcssResponse;
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in ConvertXMLResponseToString: " + ex);
                return null;
            }

        }

        public static string Sign(string msg, string date)
        {
            try
            {
                pspCertificate = new Security(MpcssMessageConstants.PspFilePath + MpcssMessageConstants.pspCertificateFile, MpcssMessageConstants.PrivateKeyToken, MpcssMessageConstants.HashAlgorithm);
                string message = $"{msg}{date}";
                return pspCertificate.Sign(message);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static bool VerifyData(string msg, string signature)
        {
            try
            {
                mpcCertificate = new Security(MpcssMessageConstants.PspFilePath + MpcssMessageConstants.mpcCertificateFile, MpcssMessageConstants.HashAlgorithm);
                return mpcCertificate.Verify(msg, signature);
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static Dictionary<string, string> PrepareEnvelopeAttributes(MqMessage message)
        {
            var dictionary = new Dictionary<string, string>();
            string str1;
            string str2;
            switch (message.MessageType)
            {
                case "cstmrreg.01.01":
                    str1 = "CSTMRREG.01";
                    str2 = "MX";
                    break;
                case "cstmrreg.02.01":
                    str1 = "CSTMRREG.02";
                    str2 = "MX";
                    break;
                case "cstmrreg.03.01":
                    str1 = "CSTMRREG.03";
                    str2 = "MX";
                    break;
                case "cstmrreg.06.01":
                    str1 = "CSTMRREG.06";
                    str2 = "MX";
                    break;
                case "cstmrreg.07.01":
                    str1 = "CSTMRREG.07";
                    str2 = "MX";
                    break;
                case "cstmrreg.08.01":
                    str1 = "CSTMRREG.08";
                    str2 = "MX";
                    break;
                case "pacs.008.001.05":
                    str1 = "PACS.008";
                    str2 = "MX";
                    break;
                case "cstmrreg.10.01":
                    str1 = "CSTMRREG.10";
                    str2 = "MX";
                    break;
                case "pacs.003.001.05":
                    str1 = "PACS.003";
                    str2 = "MX";
                    break;
                case "pacs.002.001.06":
                    str1 = "PACS.002";
                    str2 = "MX";
                    break;
                case "pacs.028.001.01":
                    str1 = "PACS.028";
                    str2 = "MX";
                    break;
                case "pacs.004.001.05":
                    str1 = "PACS.004";
                    str2 = "MX";
                    break;
                case "cstmrreg.20.01":
                    str1 = "CSTMRREG.20";
                    str2 = "MX";
                    break;
                case "cstmrreg.25.01":
                    str1 = "CSTMRREG.25";
                    str2 = "MX";
                    break;
                default:
                    throw new Exception(string.Format("No message type found for {0}", (object)message.MessageType));
            }
            dictionary.Add("messageType", str1);
            dictionary.Add("messageFormat", str2);
            return dictionary;
        }

        public static MqMessage WrapMessage(MqMessage message, string date, Dictionary<string, string> attributes)
        {
            try
            {
                attributes.TryGetValue("messageType", out var str1);
                attributes.TryGetValue("messageFormat", out var str2);
                var stringBuilder = new StringBuilder();
                stringBuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                stringBuilder.Append("<ps:envelope xmlns:ps=\"urn:ats:mpc:envelope\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
                stringBuilder.Append($"<id>{message.MessageId}</id>");
                stringBuilder.Append($"<type>{str1}</type>");
                stringBuilder.Append($"<format>{str2}</format>");
                stringBuilder.Append($"<date>{date}</date>");
                stringBuilder.Append($"<signature>{message.Signature}</signature>");
                stringBuilder.Append($"<content><![CDATA[{message.Contents}]]></content>");
                stringBuilder.Append("</ps:envelope>");
                
                message.Contents = (object)stringBuilder.ToString().Replace("\r\n", "").Replace("\r", "");
                message.Headers.Clear();
                return message;
            }
            catch (Exception ex)
            {
                //Log.Information(e.StackTrace);
                //Log.Information("Connection error : " + e);
                return null;
            }

        }

        public static MessageRequesitesDto PopulateMessageType(MPCSSRecordRequest mpcssMessageType)
        {
            var message = new MessageRequesitesDto();
            switch (mpcssMessageType)
            {
                case MPCSSRecordRequest.RecordOpeningRequest:
                    message.MessageType = MPCSSMessageTypes.OPEN_CUSTOMER_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
                case MPCSSRecordRequest.RecordMaintenanceRequest:
                    message.MessageType = MPCSSMessageTypes.MAINTAIN_CUSTOMER_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
                case MPCSSRecordRequest.RecordClosingRequest:
                    message.MessageType = MPCSSMessageTypes.CLOSE_CUSTOMER_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
                case MPCSSRecordRequest.AccountOpeningRequest:
                    message.MessageType = MPCSSMessageTypes.OPEN_ACCOUNT_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
                case MPCSSRecordRequest.AccountMaintenanceRequest:
                    message.MessageType = MPCSSMessageTypes.MAINTAIN_ACCOUNT_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
                case MPCSSRecordRequest.AccountClosingRequest:
                    message.MessageType = MPCSSMessageTypes.CLOSE_ACCOUNT_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
                case MPCSSRecordRequest.PaymentOutwardCreditRequest:
                    message.MessageType = MPCSSMessageTypes.CREDIT_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.OutwardPaymentQueue;
                    message.MessageCategory = MessageCategory.PaymentRequest;
                    break;
                case MPCSSRecordRequest.PaymentOutwardDebitRequest:
                    message.MessageType = MPCSSMessageTypes.DEBIT_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.OutwardPaymentQueue;
                    message.MessageCategory = MessageCategory.PaymentRequest;
                    break;
                case MPCSSRecordRequest.PaymentReturnRequest:
                    message.MessageType = MPCSSMessageTypes.PAYMENT_RETURN_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.OutwardPaymentQueue;
                    message.MessageCategory = MessageCategory.PaymentRequest;
                    break;
                case MPCSSRecordRequest.PaymentStatusReport:
                    message.MessageType = MPCSSMessageTypes.PAYMENT_STATUS_REPORT_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.OutwardReplyQueue;
                    message.MessageCategory = MessageCategory.PaymentResponse;
                    break;
                case MPCSSRecordRequest.PaymentEnquiry:
                    message.MessageType = MPCSSMessageTypes.PAYMENT_ENQUIRY_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.PaymentEnquiryRequestQueue;
                    message.MessageCategory = MessageCategory.PaymentEnquiry;
                    break;
                case MPCSSRecordRequest.CustomerNameVerificationRequest:
                    message.MessageType = MPCSSMessageTypes.CUSTOMER_NAME_VERIFICATION_REQUEST_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.CustomerNameRequestQueue;
                    message.MessageCategory = MessageCategory.CustomerNameVerification;
                    break;
                case MPCSSRecordRequest.DefaultAccountVerificationRequest:
                    message.MessageType = MPCSSMessageTypes.DEFAULT_ACCOUNT_VERIFICATION_REQUEST_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.CheckDefaultRequestQueue;
                    message.MessageCategory = MessageCategory.DefaultAccountVerification;
                    break;
                case MPCSSRecordRequest.RegistrationResponse:
                    message.MessageType = MPCSSMessageTypes.REGISTRATION_RESPONSE_MESSAGE_TYPE;
                    message.QueueType = MPCSSQueues.RegistrationRequestQueue;
                    message.MessageCategory = MessageCategory.RegistrationRequest;
                    break;
            }

            return message;
        }

        public static Envelope BuildResponseEnvelope(string responseXml)
        {
            try
            {
                if (responseXml != null && responseXml.Contains("envelope"))
                {
                    int envelopeStartIndex = responseXml.IndexOf("<ps:envelope");
                    int envelopeEndIndex = responseXml.IndexOf("</ps:envelope>");
                    responseXml = responseXml.Substring(envelopeStartIndex, envelopeEndIndex - envelopeStartIndex);
                    if (responseXml != null)
                    {
                        envelopeStartIndex = responseXml.IndexOf("<ps:envelope");
                        var envelopeHead = responseXml.Substring(envelopeStartIndex, responseXml.IndexOf("<id>") - envelopeStartIndex);
                        if (responseXml != null)
                        {
                            responseXml = responseXml.Replace(envelopeHead, "");
                            responseXml = "<envelope>" + responseXml + "</envelope>";
                        }
                    }
                    System.Diagnostics.Debug.WriteLine("Message from  function inside envelope if loop [{0}]", responseXml);
                    Console.WriteLine("Message from function inside envelope if loop[{0}]", responseXml);
                    Envelope envelope = (Envelope) ConvertXMLResponseToString<Envelope>(responseXml);
                    return envelope;
                }
                return null;
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in building response envelope: " + ex);
                return null;
            }
        }

        public static string BuildAchNs2Data(string xmlRequest)
        {
            try
            {
                if (xmlRequest != null && xmlRequest.Contains("<achSupplementaryData>"))
                {
                    xmlRequest = xmlRequest.Replace("achSupplementaryData", "ns2:achSupplementaryData");
                }
                if (xmlRequest != null && xmlRequest.Contains("<batchSource>"))
                {
                    xmlRequest = xmlRequest.Replace("batchSource", "ns2:batchSource");
                }
                if (xmlRequest != null && xmlRequest.Contains("<sessionSequence>"))
                {
                    xmlRequest = xmlRequest.Replace("sessionSequence", "ns2:sessionSequence");
                }
                if (xmlRequest != null && xmlRequest.Contains("<Dbtr />"))
                {
                    xmlRequest = xmlRequest.Replace("<Dbtr />", "<Dbtr></Dbtr>");
                }
                if (xmlRequest != null && xmlRequest.Contains("<Cdtr />"))
                {
                    xmlRequest = xmlRequest.Replace("<Cdtr />", "<Cdtr></Cdtr>");
                }

                return xmlRequest;
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in BuildAchNs2Data: " + ex);
                return null;
            }

        }

        public static string BuildAchNs2ResponseData(string xmlRequest)
        {
            try
            {
                if (xmlRequest != null && xmlRequest.Contains("ns2:achSupplementaryData"))
                {
                    xmlRequest = xmlRequest.Replace("ns2:achSupplementaryData", "achSupplementaryData");
                }
                if (xmlRequest != null && xmlRequest.Contains("ns2:batchSource"))
                {
                    xmlRequest = xmlRequest.Replace("ns2:batchSource", "batchSource");
                }
                if (xmlRequest != null && xmlRequest.Contains("ns2:sessionSequence"))
                {
                    xmlRequest = xmlRequest.Replace("ns2:sessionSequence", "sessionSequence");
                }

                return xmlRequest;
            }
            catch (Exception ex)
            {
                //Log.Information("ERROR in BuildAchNs2ResponseData: " + ex);
                return null;
            }

        }
    }
}
