using APG.MessageQueue.Mpcss.ActiveMQTransport;
using APGDigitalIntegration.DomainHelper.ViewModels;
using APGMPCSSIntegration.Common.CommomMethods.MessageBuilders;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using Newtonsoft.Json;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Common.OriginalMessageIdentifiers;
using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGDigitalIntegration.Constant;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Communicators;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;

namespace APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter
{
    public class SimulateHostAdapter : ISimulateHostAdapter
    {
        private readonly ILoggingService _loggingService;
        private readonly IMpcssCommunicator _mpcssCommunicator;
        public SimulateHostAdapter(IMpcssCommunicator mpcssCommunicator, ILoggingService loggingService)
        {
            _mpcssCommunicator = mpcssCommunicator;
            _loggingService = loggingService;
        }

        public async Task Insert(SimulateLogViewModel log)
        {
            log.LogOperation = LogOperation.Insert;
            await _loggingService.LogSimulate(log);
        }

        public void Search(SimulateLogViewModel log)
        {
            log.LogOperation = LogOperation.Search;
            _loggingService.SearchSimulateLog(log);
        }

        public async Task SimulateLogResponse(SimulateLogViewModel log)
        {
            MqMessage? message = null;
            switch (log.Type)
            {
                case "RecordOpeningRequest":
                case "RecordMaintenanceRequest":
                case "RecordClosingRequest":
                    message = await MPCSSRecordResponse(log);
                    break;
                case "AccountOpeningRequest":
                case "AccountMaintenanceRequest":
                case "AccountClosingRequest":
                    message = await MPCSSAccountResponse(log);
                    break;
            }
            if (message != null)
            {
                Task.Run(() =>
                {
                    _mpcssCommunicator.SendMessage(message, MPCSSQueues.RegistrationResponseQueue, ActiveMQMessageTypes.Text);
                });
            }
        }

        public async Task<MqMessage> MPCSSRecordResponse(SimulateLogViewModel log)
        {
            await Insert(log);

            var messageType = log.Type;
            var baseInternalRequest = JsonConvert.DeserializeObject<RegistrationRecordInputDto>(log.Request);
            MPCSSRecordRequest mpcssMessageType = SharedEnums.GetValueFromDescription<MPCSSRecordRequest>(messageType);
            var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);
            //var externalRequest = ConvertToExternalRequest(baseInternalRequest, mpcssMessageType, MpcssMessage);
            RegistrationResponse registration = ConstructSimulateStatus(baseInternalRequest, log, messageType);
            var registrationResp = new RegistrationResponseRoot
            {
                RegResp = registration
            };
            var xmlRequest = MpcssMethods.ConvertRequestToXMLString(registrationResp);
            string datetime = registration.MsgId.CreatedDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, registration.OrgnlMsgId.OriginalMessageId, registration.OrgnlMsgId.OriginalMessageType, datetime);
            return message;
        }
        private async Task<MqMessage> MPCSSAccountResponse(SimulateLogViewModel log)
        {
            await Insert(log);

            var messageType = log.Type;
            var baseInternalRequest = JsonConvert.DeserializeObject<RegistrationAccountInputDto>(log.Request);
            MPCSSRecordRequest mpcssMessageType = SharedEnums.GetValueFromDescription<MPCSSRecordRequest>(messageType);
            var MpcssMessage = MpcssMethods.PopulateMessageType(mpcssMessageType);
            //var externalRequest = ConvertToExternalAccountRequest(baseInternalRequest, mpcssMessageType, MpcssMessage);
            RegistrationResponse registration = ConstructSimulateAccountStatus(baseInternalRequest, log, messageType);
            var registrationResp = new RegistrationResponseRoot
            {
                RegResp = registration
            };
            var xmlRequest = MpcssMethods.ConvertRequestToXMLString(registrationResp);
            string datetime = registration.MsgId.CreatedDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, registration.OrgnlMsgId.OriginalMessageId, registration.OrgnlMsgId.OriginalMessageType, datetime);
            return message;
        }
        public RegistrationResponse ConstrutRegistrationResponse(MqMessage resp)
        {
            var regResponse = new RegistrationResponse();
            string responseXml = (string)resp.Contents; // ((ITextMessage)resp).Text;
            Envelope envelope = MpcssMethods.BuildResponseEnvelope(responseXml);
            bool isVerified = false;
            if (envelope != null)
            {
                if (envelope.DigitalSignature != null && envelope.DigitalSignature.Length > 0)
                {
                    DateTime dateObject = DateTime.Parse(envelope.TransactionDate);
                    string dateString = dateObject.ToString("yyyy-MM-ddTHH:mm:ss");
                    string Message = envelope.MessageContent + dateString;
                    isVerified = MpcssMethods.VerifyData(Message, envelope.DigitalSignature);
                }
                if (responseXml != null && responseXml.Contains("RegResp"))
                {
                    int startIndex = responseXml.IndexOf("<RegResp>");
                    int endIndex = responseXml.IndexOf("</Document>");
                    responseXml = responseXml.Substring(startIndex, endIndex - startIndex);
                }
            }
            RegistrationResponse response = (RegistrationResponse)MpcssMethods.ConvertXMLResponseToString<RegistrationResponse>(responseXml);
            return response;
        }

        private RegistrationResponse ConstructSimulateStatus(RegistrationRecordInputDto baseInternalRequest, SimulateLogViewModel log, string messageType)
        {
            var response = new RegistrationResponse();
            response.MsgId = new MessageIdentification()
            {
                Id = log.RefId,
                CreatedDateTime = DateTime.Now
            };
            response.OrgnlMsgId = new OriginalMessageIdentifier()
            {
                OriginalMessageId = log.RefId,

            };
            var msgStatus = new OriginalMessageStatus();
            msgStatus.Status = MPCSSResponseStatus.ACPT.ToString();
            msgStatus.ReasonCode = MPCSSResponseReasonCode.ProcessedSuccessfully.ToString();
            msgStatus.Narration = MPCSSResponseReasonCode.ProcessedSuccessfully.GetEnumDescription();
            var recordExists = log.Id != Guid.Empty;
            switch (messageType)
            {
                case "RecordOpeningRequest":
                    response.OrgnlMsgId.OriginalMessageType = "cstmrreg.01.01";
                    if (string.IsNullOrEmpty(baseInternalRequest.CustomerId) || string.IsNullOrEmpty(baseInternalRequest.IdentificationTypeCode) || string.IsNullOrEmpty(baseInternalRequest.IdIssuingCountryCode))
                    {
                        msgStatus.Status = MPCSSResponseStatus.RJCT.ToString();
                        msgStatus.ReasonCode = MPCSSResponseReasonCode.InvalidIdFormat.ToString();
                        msgStatus.Narration = MPCSSResponseReasonCode.InvalidIdFormat.GetEnumDescription();
                    }
                    else if (!string.IsNullOrEmpty(baseInternalRequest.CustomerId) && recordExists)
                    {
                        msgStatus.Status = MPCSSResponseStatus.RJCT.ToString();
                        msgStatus.ReasonCode = MPCSSResponseReasonCode.AliasAlreadyUsed.ToString();
                        msgStatus.Narration = MPCSSResponseReasonCode.AliasAlreadyUsed.GetEnumDescription();
                    }
                    break;
                case "RecordMaintenanceRequest":
                    response.OrgnlMsgId.OriginalMessageType = "cstmrreg.02.01";
                    if (string.IsNullOrEmpty(baseInternalRequest.CustomerId) || string.IsNullOrEmpty(baseInternalRequest.IdentificationTypeCode) || string.IsNullOrEmpty(baseInternalRequest.IdIssuingCountryCode))
                    {
                        msgStatus.Status = MPCSSResponseStatus.RJCT.ToString();
                        msgStatus.ReasonCode = MPCSSResponseReasonCode.InvalidIdFormat.ToString();
                        msgStatus.Narration = MPCSSResponseReasonCode.InvalidIdFormat.GetEnumDescription();
                    }
                    else if (!recordExists)
                    {
                        msgStatus.Status = MPCSSResponseStatus.RJCT.ToString();
                        msgStatus.ReasonCode = MPCSSResponseReasonCode.InvalidAccount.ToString();
                        msgStatus.Narration = MPCSSResponseReasonCode.InvalidAccount.GetEnumDescription();
                    }
                    break;
                case "RecordClosingRequest":
                    response.OrgnlMsgId.OriginalMessageType = "cstmrreg.03.01";
                    if (string.IsNullOrEmpty(baseInternalRequest.CustomerId) || string.IsNullOrEmpty(baseInternalRequest.IdentificationTypeCode) || string.IsNullOrEmpty(baseInternalRequest.IdIssuingCountryCode))
                    {
                        msgStatus.Status = MPCSSResponseStatus.RJCT.ToString();
                        msgStatus.ReasonCode = MPCSSResponseReasonCode.InvalidIdFormat.ToString();
                        msgStatus.Narration = MPCSSResponseReasonCode.InvalidIdFormat.GetEnumDescription();
                    }
                    else if (!recordExists)
                    {
                        msgStatus.Status = MPCSSResponseStatus.RJCT.ToString();
                        msgStatus.ReasonCode = MPCSSResponseReasonCode.InvalidAccount.ToString();
                        msgStatus.Narration = MPCSSResponseReasonCode.InvalidAccount.GetEnumDescription();
                    }
                    break;
                default:
                    break;
            }
            response.OrgnlMsgSts = msgStatus;
            return response;
        }

        private RegistrationResponse ConstructSimulateAccountStatus(RegistrationAccountInputDto baseInternalRequest, SimulateLogViewModel log, string messageType)
        {
            var response = new RegistrationResponse();
            response.MsgId = new MessageIdentification()
            {
                Id = log.RefId,
                CreatedDateTime = DateTime.Now
            };
            response.OrgnlMsgId = new OriginalMessageIdentifier()
            {
                OriginalMessageId = log.RefId,

            };
            var msgStatus = new OriginalMessageStatus();
            msgStatus.Status = MPCSSResponseStatus.ACPT.ToString();
            msgStatus.ReasonCode = MPCSSResponseReasonCode.ProcessedSuccessfully.ToString();
            var recordExists = log.Id != Guid.Empty;
            switch (messageType)
            {
                case "AccountOpeningRequest":
                    response.OrgnlMsgId.OriginalMessageType = "cstmrreg.06.01";
                    break;
                case "AccountMaintenanceRequest":
                    response.OrgnlMsgId.OriginalMessageType = "cstmrreg.07.01";
                    break;
                case "AccountClosingRequest":
                    response.OrgnlMsgId.OriginalMessageType = "cstmrreg.08.01";
                    break;
                default:
                    break;
            }
            response.OrgnlMsgSts = msgStatus;
            return response;
        }
        

        //private MqMessage ConvertToExternalRequest(RegistrationRecordInputDto mpcssRequest, MPCSSRecordRequest MessageType, MessageRequesitesDto messageRequesites)
        //{
        //    string xmlRequest = string.Empty;
        //    string date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        //    string DateOfBirth = mpcssRequest.DobOrRegistrationDate.ToString("yyyy-MM-dd");

        //    if (MessageType.Equals(MPCSSRecordRequest.RecordOpeningRequest))
        //    {
        //        var CustomerRegRequest = new CustomerRegistrationDto
        //        {
        //            MsgId = new MessageIdentifier
        //            {
        //                Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
        //                CreatedDateTime = date,
        //            },
        //            PrtcpntId = mpcssRequest.ParticipantId,
        //            CstmrInfo = new CustomerInformationDto
        //            {
        //                CstmrId = new CustomerIdentification
        //                {
        //                    Id = mpcssRequest.CustomerId,
        //                    IdIssngCtryCd = mpcssRequest.IdIssuingCountryCode,
        //                    IdTp = mpcssRequest.IdentificationTypeCode
        //                },

        //                CstmrNm = mpcssRequest.CustomerName,
        //                DobOrRegDt = DateOfBirth
        //            },
        //            Address = new AddressInformationDto
        //            {
        //                BldgNb = mpcssRequest.BuildingNumber,
        //                CtryCd = mpcssRequest.CountryCode,
        //                StrtNm = mpcssRequest.StreetName,
        //                CtyNm = mpcssRequest.CountryCode,
        //                PstCd = mpcssRequest.PostalCode,
        //                GvrnNm = mpcssRequest.GovernorateName,
        //                MobNb = mpcssRequest.MobileNumberIdentification,
        //                PhneNb = mpcssRequest.PhoneNumber,
        //                POBx = mpcssRequest.POBox,
        //                TwnNm = mpcssRequest.TownName
        //            },
        //            AdtnlInf = mpcssRequest.AdditionalInfo,
        //        };
        //        var request = new RegistrationRequestsDto
        //        {
        //            CstmrOpngReq = CustomerRegRequest
        //        };
        //        xmlRequest = MpcssMethods.ConvertRequestToXMLString(request);
        //    }
        //    else if (MessageType.Equals(MPCSSRecordRequest.RecordMaintenanceRequest))
        //    {
        //        var CustomerRegRequest = new CustomerRegistrationDto
        //        {
        //            MsgId = new MessageIdentifier
        //            {
        //                Id = Guid.NewGuid().ToString("N"),
        //                CreatedDateTime = date,
        //            },
        //            PrtcpntId = mpcssRequest.ParticipantId,
        //            CstmrInfo = new CustomerInformationDto
        //            {
        //                CstmrId = new CustomerIdentification
        //                {
        //                    Id = mpcssRequest.CustomerId,
        //                    IdIssngCtryCd = mpcssRequest.IdIssuingCountryCode,
        //                    IdTp = mpcssRequest.IdentificationTypeCode
        //                },
        //                CstmrNm = mpcssRequest.CustomerName,
        //                DobOrRegDt = DateOfBirth
        //            },
        //            Address = new AddressInformationDto
        //            {
        //                BldgNb = mpcssRequest.BuildingNumber,
        //                CtryCd = mpcssRequest.CountryCode,
        //                StrtNm = mpcssRequest.StreetName,
        //                CtyNm = mpcssRequest.CountryCode,
        //                PstCd = mpcssRequest.PostalCode,
        //                GvrnNm = mpcssRequest.GovernorateName,
        //                MobNb = mpcssRequest.MobileNumberIdentification,
        //                PhneNb = mpcssRequest.PhoneNumber,
        //                POBx = mpcssRequest.POBox,
        //                TwnNm = mpcssRequest.TownName
        //            },
        //            AdtnlInf = mpcssRequest.AdditionalInfo
        //        };
        //        var request = new RegistrationRequestsDto
        //        {
        //            CstmrMntncReq = CustomerRegRequest
        //        };
        //        xmlRequest = MpcssMethods.ConvertRequestToXMLString(request);
        //    }
        //    else if (MessageType.Equals(MPCSSRecordRequest.RecordClosingRequest))
        //    {
        //        var CustomerCloseRequest = new CustomerClosingRequest
        //        {
        //            MsgId = new MessageIdentifier
        //            {
        //                Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
        //                CreatedDateTime = date,
        //            },
        //            PrtcpntId = mpcssRequest.ParticipantId,
        //            CstmrId = new CustomerIdentification
        //            {
        //                Id = mpcssRequest.CustomerId,
        //                IdIssngCtryCd = mpcssRequest.IdIssuingCountryCode,
        //                IdTp = mpcssRequest.IdentificationTypeCode
        //            },
        //            AdtnlInf = mpcssRequest.AdditionalInfo
        //        };
        //        var request = new RegistrationRequestsDto
        //        {
        //            CstmrClsgReq = CustomerCloseRequest
        //        };
        //        xmlRequest = MpcssMethods.ConvertRequestToXMLString(request);
        //    }
        //    MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, mpcssRequest.MessageIdentifier.MessageIdentificationCode, messageRequesites.MessageType, date);
        //    return message;
        //}

        //private ServiceResponse ConvertToExternalAccountRequest(RegistrationAccountInputDto mpcssRequest, string msgId, MqMessage request, MPCSSRecordRequest messageType)
        //{
        //    // Construction of response message for active mq 
        //    RegistrationResponse registration = null;
        //    if (messageType.Equals(MPCSSRecordRequest.AccountOpeningRequest))
        //    {
        //        string status = "ACPT", reasonCode = "1000", narration = string.Empty;
        //        string requestMobileNo = CommonMethods.GetXMLAttributeValue(XElement.Parse(Convert.ToString(request.Contents)), "MblOrSvc");

        //        // if mobile no is 80000 then its a closed customer or when 90000 it doesn't exists
        //        if (requestMobileNo.Equals("8004"))
        //        {
        //            status = "RJCT";
        //            reasonCode = "1001";
        //            narration = "Account identifier is already used by another customer";
        //        }

        //        registration = new RegistrationResponse()
        //        {
        //            MsgId = new MessageIdentification()
        //            {
        //                Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
        //                CreatedDateTime = DateTime.Now
        //            },
        //            OrgnlMsgId = new OriginalMessageIdentification()
        //            {
        //                OriginalMsgId = "AWPY02092020004",
        //                OriginalMessageType = "cstmrreg.06.01"
        //            },
        //            OrgnlMsgSts = new OriginalMessageStatus()
        //            {
        //                Status = status,
        //                ReasonCode = reasonCode,
        //                Narration = narration
        //            }
        //        };
        //    }
        //    else if (messageType.Equals(MPCSSRecordRequest.AccountMaintenanceRequest))
        //    {
        //        string status = "ACPT", reasonCode = "1000", narration = string.Empty;
        //        string requestMobileNo = CommonMethods.GetXMLAttributeValue(XElement.Parse(request.Contents.ToString()), "MblOrSvc");

        //        // if mobile no is 80000 then its a closed customer or when 90000 it doesn't exists
        //        if (requestMobileNo.Equals("8005"))
        //        {
        //            status = "RJCT";
        //            reasonCode = "1001";
        //            narration = "No open account was found";
        //        }

        //        registration = new RegistrationResponse()
        //        {
        //            MsgId = new MessageIdentification()
        //            {
        //                Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
        //                CreatedDateTime = DateTime.Now
        //            },
        //            OrgnlMsgId = new OriginalMessageIdentification()
        //            {
        //                OriginalMsgId = "AWPY010920200006",
        //                OriginalMessageType = "cstmrreg.07.01"
        //            },
        //            OrgnlMsgSts = new OriginalMessageStatus()
        //            {
        //                Status = status,
        //                ReasonCode = reasonCode,
        //                Narration = narration
        //            }
        //        };
        //    }
        //    else if (messageType.Equals(MPCSSRecordRequest.AccountClosingRequest))
        //    {
        //        registration = new RegistrationResponse()
        //        {
        //            MsgId = new MessageIdentification()
        //            {
        //                Id = mpcssRequest.MessageIdentifier.MessageIdentificationCode,
        //                CreatedDateTime = DateTime.Now
        //            },
        //            OrgnlMsgId = new OriginalMessageIdentification()
        //            {
        //                OriginalMsgId = "AWPY01092020005",
        //                OriginalMessageType = "cstmrreg.08.01"
        //            },
        //            OrgnlMsgSts = new OriginalMessageStatus()
        //            {
        //                Status = "ACPT",
        //                ReasonCode = "1000"
        //            }
        //        };
        //    }

        //    var registrationResp = new RegistrationResponseDto
        //    {
        //        RegResp = registration
        //    };

        //    var xmlRequest = MpcssMethods.ConvertRequestToXMLString(registrationResp);
        //    string datetime = registration.MsgId.CreatedDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
        //    MqMessage message = MessageBuilder.ConstructMqMessage(xmlRequest, registration.OrgnlMsgId.OriginalMsgId, registration.OrgnlMsgId.OriginalMessageType, datetime);

        //    Task.Run(() =>
        //    {
        //        _mpcssCommunicator.PublishMessage(message, QueueConstants.RegistrationResponseQueue);
        //    });

        //    return new ServiceResponse(
        // success: true,
        // code: ResponseCodes.Success,
        // description: _messageHandler.GetMessage(PaymentSuccessMessage.RequestSentSuccessfully));

        //}
    }
}
