using APGDigitalIntegration.Application.Interfaces;
using APGMPCSSIntegration.Constant;
using System.Diagnostics.CodeAnalysis;
using APG.MessageQueue.Contracts.CommunicationLog;
using APG.MessageQueue.Contracts.MerchantMPCSSOperations;
using APG.MessageQueue.Interfaces;
using APG.MessageQueue.Mpcss.Interfaces;
using Moq;
using APGDigitalIntegration.DomainHelper.ViewModels;
using Newtonsoft.Json;
using APGMPCSSIntegration.Common.CommonViewModels.Response;
using APGDigitalIntegration.Common.CommonServices;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGDigitalIntegration.Application.Services;
using APGDigitalIntegration.Common.CommonMethods.MessageBuilders;
using APGDigitalIntegration.Common.CommonViewModels.Operation.Registration.Response;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace APGDigitalIntegration.Application.Test
{
    [ExcludeFromCodeCoverage]
    public class MerchantMPCSSRecordTests
    {
        [Fact]
        public async Task MerchantRegistration()
        {
            var req = MPCSSTestData();
            req.RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest;
            req.Request.IdentNumber = new Random().Next(10000, 99999).ToString();
            var internalResponse = await MpcssRequest(req);
            Assert.NotNull(internalResponse);
            Assert.NotNull(internalResponse.OrgnlMsgSts);
            Assert.True(internalResponse.OrgnlMsgSts.Status == MPCSSResponseStatus.ACPT.GetEnumDescription());
        }

        
        [Fact]
        public async Task MerchantRegistrationExistingMerchant()
        {
            var req = MPCSSTestData();
            req.RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest;
            var registered = await MpcssRequest(req);
            var internalResponse = await MpcssRequest(req);
            Assert.NotNull(internalResponse);
            Assert.NotNull(internalResponse.OrgnlMsgSts);
            Assert.True(internalResponse.OrgnlMsgSts.ReasonCode == MPCSSResponseReasonCode.AliasAlreadyUsed.GetEnumDescription());
            Assert.True(internalResponse.OrgnlMsgSts.Status == MPCSSResponseStatus.RJCT.GetEnumDescription());
        }

        [Fact]
        public async Task MerchantRegistrationforInvalidMerchant()
        {
            var req = MPCSSTestData();
            req.RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest;
            var internalResponse = await MpcssRequest(req);
            req.Request.IdentNumber = String.Empty;
            Assert.NotNull(internalResponse);
            Assert.NotNull(internalResponse.OrgnlMsgSts);
            Assert.True(internalResponse.OrgnlMsgSts.ReasonCode == MPCSSResponseReasonCode.InvalidIdFormat.GetEnumDescription());
            Assert.True(internalResponse.OrgnlMsgSts.Status == MPCSSResponseStatus.RJCT.GetEnumDescription());
        }

        [Fact]
        public async Task ModifyMerchantRegistration()
        {
            var req = MPCSSTestData();
            req.RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest;
            var registered = await MpcssRequest(req);
            req.RequestType = (int)MPCSSRecordRequest.AccountMaintenanceRequest;
            var internalResponse = await MpcssRequest(req);
            Assert.NotNull(internalResponse);
            Assert.NotNull(internalResponse.OrgnlMsgSts);
            Assert.True(internalResponse.OrgnlMsgSts.Status == MPCSSResponseStatus.ACPT.GetEnumDescription());
        }

        [Fact]
        public async Task ModifyMerchantRegistrationDeletedMerchant()
        {
            var req = MPCSSTestData();
            req.RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest;
            var registered = await MpcssRequest(req);
            req.RequestType = (int)MPCSSRecordRequest.AccountClosingRequest;
            var deleted = await MpcssRequest(req);

            req.RequestType = (int)MPCSSRecordRequest.AccountMaintenanceRequest;
            var internalResponse = await MpcssRequest(req);

            Assert.NotNull(internalResponse);
            Assert.NotNull(internalResponse.OrgnlMsgSts);
            Assert.True(internalResponse.OrgnlMsgSts.ReasonCode == MPCSSResponseReasonCode.AccountIsClosedOrBlocked.GetEnumDescription());
            Assert.True(internalResponse.OrgnlMsgSts.Status == MPCSSResponseStatus.RJCT.GetEnumDescription());
        }

        [Fact]
        public async Task CloseMerchantRegistration()
        {
            var req = MPCSSTestData();
            req.RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest;
            var merchantOperationAppService = new Mock<IMerchantMPCSSOperationAppService>();
            merchantOperationAppService.Setup(s => s.MerchantMPCSSOperationRequest(req));
        }
        public void Dispose()
        {
            GC.Collect();
        }

        private UpdateMpcssMerchant MPCSSTestData()
        {
            var internalRequest = new UpdateMpcssMerchant
            {
                MsgId = Guid.NewGuid().ToString("N"),
                RequestType = (int)MPCSSRecordRequest.RecordOpeningRequest,
                Request = new MpcssMerchantModel()
                {
                    IdentCode = "CRNO",
                    IdentNumber = "777",
                    IdentCountryCode = "OM",
                    MerchantId = "1",
                    MerchantName = "LuLu",
                    ParticipantId = "SCB002",
                    RegistrationCode = "",
                    AccountCurrency = "OMR",
                    IsAccountBanked = "rue",
                    BankId = 1,
                    AccountAlias = String.Empty,
                    POBox = "111",
                    PostalCode = "112",
                    StreetName = "Sultan Qaboos St",
                    BuildingNumber = "Boushar",
                    PhoneNumber = "+968 24 584333",
                    MobileNumber = "+968 99 6996996",
                    CityName = "Muscat",
                    TownName = "",
                    GovernorateName = "",
                    CountryCode = "OM",
                    AdditionalInfo = ""
                }
            };
            return internalRequest;
        }
        private async Task<RegistrationResponse> MpcssRequest(UpdateMpcssMerchant req)
        {
            var mpcssCommunicator = new Mock<IMpcssCommunicator>();
            var confParamHelperService = new Mock<IConfParamHelperService>();
            var mpcssCommunicationLogService = new Mock<IMPCSSCommunicationLogService>();

            AddDigitalCommunicationLog communicationLogModel = new AddDigitalCommunicationLog();
            mpcssCommunicationLogService.SetupGet(x => x.MPCSSCommunicationLogModel).Returns(communicationLogModel);

            var messageQueue = new Mock<IMessageQueue>();
            var loggingService = new Mock<ILoggingService>();
            var simulateHostAdapter = new Mock<ISimulateHostAdapter>();
            var mpcssMessageBuilder = new Mock<IMPCSSMessageBuilder>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            var merchantOperationAppService = new MerchantMPCSSOperationAppService(mpcssCommunicator.Object, confParamHelperService.Object, mpcssCommunicationLogService.Object, loggingService.Object, messageQueue.Object, simulateHostAdapter.Object, mpcssMessageBuilder.Object, serviceProviderMock.Object);
            var externalReq = merchantOperationAppService.ConstructRecordRequest(req);

            // var messageHandler = new Mock<ResponseMessageHandler>();
            // var recordHostAdapter = new RecordRegistrationHostAdapter(mpcssCommunicator.Object, confParamHelperService.Object, messageHandler.Object, mpcssCommunicationLogService.Object, simulateHostAdapter.Object);
            // await recordHostAdapter.ConstructSimulatedResponse(externalReq);


            var log = new SimulateLogViewModel();
            log.Id = Guid.NewGuid();
            log.Type = ((MPCSSRecordRequest)req.RequestType).GetEnumDescription();
            log.Data = req.Request.IdentCode + req.Request.IdentNumber + req.Request.CountryCode;
            log.RefId = req.MsgId;
            log.Request = JsonConvert.SerializeObject(externalReq);

            var logService = new Mock<ILoggingService>();
            var mpcssComm = new Mock<IMpcssCommunicator>();
            var service = new SimulateHostAdapter(mpcssComm.Object, logService.Object);
            var mpcssResponse = service.MPCSSRecordResponse(log);
            var internalResponse = service.ConstrutRegistrationResponse(mpcssResponse.Result);
            return internalResponse;
        }

    }
}