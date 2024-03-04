using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Application.Services.Messages;
using APGMPCSSIntegration.Common.CommonViewModels.Request;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.Services.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using APGDigitalIntegration.Common.CommonViewModels.Request;
using APGDigitalIntegration.IAL.Internal.Viewmodel.QR;
using APGDigitalIntegration.Application.ViewModels;

namespace APGDigitalIntegration.Services.Api.Controllers
{
    [AllowAnonymous]
    [Route(ServiceName.Controllers.Simulation)]
    public class SimulationController : ApiController
    {

        #region Fields

        private readonly ISimulationAppService _simulationAppService;

        #endregion

        #region Constructor
        public SimulationController(ISimulationAppService simulationAppService, CurrentUserDataViewModel currentUserDataViewModel, IHttpContextAccessor httpContextAccessor) : base(currentUserDataViewModel, httpContextAccessor)
        {
            _simulationAppService = simulationAppService;
        }

        #endregion

        #region APIs

        [HttpPost(ServiceNameDigitalIntegration.InwardCreditTransaction)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> InwardCreditTransaction([FromBody] InwardCreditSimulationRequest request)
        {
            var result = await _simulationAppService.InwardCreditTransaction(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }
        
        [HttpPost(ServiceNameDigitalIntegration.InwardCreditTransactionByWalletId)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> InwardCreditTransaction(QROrderSimulationRequest request)
        {
            var result = await _simulationAppService.InwardCreditTransaction(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }

        [HttpPost(ServiceNameDigitalIntegration.InwardDebitTransaction)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> InwardDebitTransaction( CreditDebitPaymentInputDto request)
        {
            var result = await _simulationAppService.InwardDebitTransaction(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }
        

        [HttpPost(ServiceNameDigitalIntegration.InwardPaymentEnquiry)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> InwardPaymentEnquiry(PaymentEnquiryInputDto request)
        {
            if (ModelState.IsValid == false)
                return CustomResponse(ModelState);

            var result = await _simulationAppService.InwardPaymentEnquiry(request);
            Response.Message = result.Message;
            Response.Success = result.Success;
            Response.ResponseCode = Convert.ToString(result.ResponseCode);
            return CustomResponse(Response);
        }




        #endregion
    }
}
