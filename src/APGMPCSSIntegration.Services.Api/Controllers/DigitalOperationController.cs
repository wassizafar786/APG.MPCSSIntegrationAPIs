using APGDigitalIntegration.Services.Api.Filters;
using APGMPCSSIntegration.Application.Interfaces;
using APGMPCSSIntegration.Application.Services.Messages;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Threading.Tasks;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.Application.ViewModels;

namespace APGMPCSSIntegration.Services.Api.Controllers
{
    [AllowAnonymous]
    [Route(ServiceName.Controllers.DigitalOperation)]
    public class DigitalOperationController : ApiController
    {
        #region Fields

        private readonly IDigitalOperationAppService _digitalOperationAppService;

        #endregion

        #region Constructor

        public DigitalOperationController(IDigitalOperationAppService digitalOperationAppService, CurrentUserDataViewModel currentUserDataViewModel, IHttpContextAccessor httpContextAccessor) : base(currentUserDataViewModel, httpContextAccessor)
        {
            _digitalOperationAppService = digitalOperationAppService;
        }

        #endregion

        #region APIs

        [HttpPost(ServiceNameDigitalIntegration.StatusReportRequest)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [EnableCommunicationLogging(LogSource.All)]
        public async Task<IActionResult> StatusReportRequest(MPCSSRecordRequest id, PaymentStatusReportInputDto request)
        {
            var result = await _digitalOperationAppService.SendPaymentStatusReportRequest(request, id);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }
        [CheckMerchantTerminal]
        [HttpPost(ServiceNameDigitalIntegration.CustomerNameVerificationRequest)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [EnableCommunicationLogging(LogSource.All)]
        public async Task<IActionResult> CustomerNameVerificationRequest(CustomerNameVerificationRequest request)
        {
            var result = await _digitalOperationAppService.SendCustomerNameVerificationRequest(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            Response.data = result.Data;
            Response.ErrorList = result.ErrorList;
            return CustomResponse(Response);
        }

        [HttpPost(ServiceNameDigitalIntegration.DefaultAccountVerificationRequest)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [EnableCommunicationLogging(LogSource.All)]
        public async Task<IActionResult> DefaultAccountVerificationRequest(MPCSSRecordRequest id, DefaultAccountInputDto request)
        {
            var result = await _digitalOperationAppService.SendDefaultAccountVerificationRequest(request, id);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }

        #endregion
    }
}
