using System;
using System.Threading;
using System.Threading.Tasks;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Application.ViewModels;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.DomainHelper.Filters;
using APGDigitalIntegration.IAL.Internal.Viewmodel;
using APGDigitalIntegration.Services.Api.Filters;
using APGMPCSSIntegration.Application.Services.Messages;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.Services.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace APGDigitalIntegration.Services.Api.Controllers
{
    [AllowAnonymous]
    [Route(ServiceName.Controllers.DigitalTransaction)]
    public class DigitalTransactionController : ApiController
    {
        #region Fields

        private readonly IDigitalTransactionAppService _digitalTransactionAppService;
        private readonly IStringLocalizer<DigitalTransactionController> _stringLocalizer;
        private readonly IDigitalTransactionReadAppService _digitalTransactionReadAppService;

        #endregion

        #region Constructor
        public DigitalTransactionController(IDigitalTransactionAppService digitalTransactionAppService, IStringLocalizer<DigitalTransactionController> stringLocalizer, IDigitalTransactionReadAppService digitalTransactionReadAppService, CurrentUserDataViewModel currentUserDataViewModel, IHttpContextAccessor httpContextAccessor) : base(currentUserDataViewModel, httpContextAccessor)
        {
            _digitalTransactionAppService = digitalTransactionAppService;
            _stringLocalizer = stringLocalizer;
            _digitalTransactionReadAppService = digitalTransactionReadAppService;
        }

        #endregion

        #region APIs

        [HttpPost(ServiceNameDigitalIntegration.CreditPaymentRequest)]
        [CheckMerchantTerminal]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreditPaymentRequest(CreditDebitPaymentInputDto request) 
        {
            if (ModelState.IsValid == false)
                return CustomResponse(ModelState);

            var result = await _digitalTransactionAppService.SendPaymentCreditRequest(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }

        [HttpPost(ServiceNameDigitalIntegration.DebitPaymentRequest)]
        [CheckMerchantTerminal]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [EnableCommunicationLogging(LogSource.All)]
        public async Task<IActionResult> DebitPaymentRequest(DebitPaymentInternalRequest internalRequest)
        {
            if (ModelState.IsValid == false)
                return CustomResponse(ModelState);

            var result = await _digitalTransactionAppService.SendPaymentDebitRequest(internalRequest, CancellationToken.None);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = result.ResponseCode;
            Response.data = result.Data;
            return CustomResponse(Response);
        }

        [HttpPost(ServiceNameDigitalIntegration.PaymentReturn)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [CheckMerchantTerminal]
        [EnableCommunicationLogging(LogSource.All)]
        public async Task<IActionResult> PaymentReturnRequest(RefundPaymentRequest request)
        {
            var result = await _digitalTransactionAppService.SendPaymentReturnRequest(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }

        [HttpPost(ServiceNameDigitalIntegration.EnquiryRequest)]
        [CheckMerchantTerminal]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [EnableCommunicationLogging(LogSource.All)]
        public async Task<IActionResult> EnquiryRequest(PaymentEnquiryRequest paymentEnquiryRequest)
        {
            if (ModelState.IsValid == false)
                return CustomResponse(ModelState);

            var result = await _digitalTransactionAppService.SendPaymentEnquiryRequest(paymentEnquiryRequest);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }

        #endregion
        
        
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [Route(ServiceNameCommon.GetByTransactionId)]
        public async Task<IActionResult> GetByTransactionId([FromBody] TransactionGetRequest transactionGetRequest)
        {
            var result = await _digitalTransactionReadAppService.GetByTransactionId(transactionGetRequest.TransactionId);

            Response.Message = _stringLocalizer[ResponseMessages.Success].Value;
            Response.Success = true;
            Response.ResponseCode = ResponseCodes.Success;
            Response.data = result;
            return CustomResponse(Response);
        }    
        
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [Route(ServiceNameCommon.GetByTransactionFilter)]
        public async Task<IActionResult> GetByTransactionFilter([FromBody] DigitalTransactionFilter digitalTransactionFilter)
        {
            var result = await _digitalTransactionReadAppService.GetByTransactionFilter(digitalTransactionFilter);

            Response.Message = _stringLocalizer[ResponseMessages.Success].Value;
            Response.Success = true;
            Response.ResponseCode = ResponseCodes.Success;
            Response.data = result;
            return CustomResponse(Response);
        }
    }
}
