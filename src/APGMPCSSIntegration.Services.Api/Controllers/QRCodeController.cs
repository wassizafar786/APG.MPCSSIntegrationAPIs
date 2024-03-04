using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Application.ViewModels;
using APGMPCSSIntegration.Application.Services.Messages;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.Services.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace APGDigitalIntegration.Services.Api.Controllers
{
    [AllowAnonymous]
    [Route(ServiceName.Controllers.QR)]
    public class QRCodeController : ApiController
    {
        #region Fields
        private readonly IQRCodeAppService _qrCodeAppService;
        #endregion

        #region Constructor

        public QRCodeController(IQRCodeAppService qrCodeAppService, CurrentUserDataViewModel currentUserDataViewModel, IHttpContextAccessor httpContextAccessor) : base(currentUserDataViewModel, httpContextAccessor)
        {
            _qrCodeAppService = qrCodeAppService;
        }
        #endregion


        #region APIs

        [HttpPost(ServiceNameDigitalIntegration.ParseISOMessage)]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> ParseISOMessage(string isoMessage)
        {
            var result = await _qrCodeAppService.ParseQR_ISO2006(isoMessage);
            Response.Message = ErrorMessage.Success;
            Response.Success = true;
            Response.data = result;
            return CustomResponse(Response);
        } 

        #endregion

    }
}
