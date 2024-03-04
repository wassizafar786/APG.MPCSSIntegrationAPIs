using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using APGDigitalIntegration.Application.ViewModels;
using APGMPCSSIntegration.Application.Services.Messages;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace APGMPCSSIntegration.Services.Api.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        private readonly ICollection<string> _errors = new List<string>();
        public readonly GenericResponse Response = new GenericResponse();
        private readonly CurrentUserDataViewModel _currentUserDataViewModel;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApiController(CurrentUserDataViewModel currentUserDataViewModel, IHttpContextAccessor httpContextAccessor)
        {
            _currentUserDataViewModel = currentUserDataViewModel;

            _httpContextAccessor = httpContextAccessor;
            GetCurrentUserDataFromToken();

        }
        protected ActionResult CustomResponse(object result = null, bool isBadRequest = false, bool isDownloadFile = false)
        {
            if (isBadRequest)
            {
                return BadRequest();
            }

            if (isDownloadFile)
            {
                return Accepted(new
                {
                    Success = true,
                    data = result
                });
            }

            if (IsOperationValid())
            {
                return Ok(result);
            }
            return BadRequest(
                new
                {
                    Success = false,
                    ErrorList = _errors,
                });
        }

        

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(e => e.Errors);
            foreach (var error in errors)
            {
                AddError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                AddError(error.ErrorMessage);
            }

            return CustomResponse();
        }

        protected ActionResult CustomResponse<T>(GenericResponse genericResponse)
        {
            if (genericResponse.Success)
                return Ok(genericResponse);

            return BadRequest(genericResponse);
        }

        protected bool IsOperationValid()
        {
            return !_errors.Any();
        }

        protected void AddError(string erro)
        {
            _errors.Add(erro);
        }

        protected void ClearErrors()
        {
            _errors.Clear();
        }
        protected void GetCurrentUserDataFromToken()
        {
            // var token = HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationrValue);
            var token = _httpContextAccessor.HttpContext.Request.Headers.Authorization;
            // var token = Request?.Headers["Authorization"];
            if (!string.IsNullOrEmpty(token))
            {
                token = token.ToString().Replace("Bearer", "");
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(token.ToString().Trim());
                var merchantrefIdStr = securityToken.Claims?.FirstOrDefault(c => c.Type == "merchantRefId") == null ? "" : securityToken.Claims?.FirstOrDefault(c => c.Type == "merchantRefId").Value;
                if (long.TryParse(merchantrefIdStr, out long merchantRefId))
                    _currentUserDataViewModel.MerchantRefId = merchantRefId;


            }

        }
    }
}
