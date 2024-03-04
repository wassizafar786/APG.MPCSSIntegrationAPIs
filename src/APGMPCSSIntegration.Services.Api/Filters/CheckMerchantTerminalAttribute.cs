using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.Application.ViewModels;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.ViewModels.APGFundamental.Terminal;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.DomainHelper;
using APGMPCSSIntegration.DomainHelper.Exceptions;
using APGMPCSSIntegration.IAL.Internal.BaseRequests;
using APGMPCSSIntegration.IAL.Internal.Communicator;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APGDigitalIntegration.Services.Api.Filters
{
    public class CheckMerchantTerminalAttribute : TypeFilterAttribute
    {
        private readonly bool _ignoreFilter;
      
        /// <summary>
        /// Create instance of the filter attribute
        /// </summary>
        /// <param name="ignore">Whether to ignore the execution of filter actions</param>
        public CheckMerchantTerminalAttribute(bool ignore = false) : base(typeof(CheckMerchantTerminalFilter))
        {
            _ignoreFilter = ignore;
          
            Arguments = new object[] { ignore };
        }

        public bool IgnoreFilter => _ignoreFilter;

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to closed store
        /// </summary>
        /// 
        private class CheckMerchantTerminalFilter : IAsyncActionFilter
        {
            #region Fields

            private readonly bool _ignoreFilter;
            private readonly ITerminalMerchantAppService _terminalMerchantAppService;
            private readonly IMPCSSCommunicationLogService _communicationLogService;
            private readonly CurrentUserDataViewModel _currentUserDataViewModel;

            #endregion

            #region Ctor
            public CheckMerchantTerminalFilter(bool ignoreFilter, ITerminalMerchantAppService terminalMerchantAppService,
                IMPCSSCommunicationLogService communicationLogService, CurrentUserDataViewModel currentUserDataViewModel)
            {
                _ignoreFilter = ignoreFilter;
                _terminalMerchantAppService = terminalMerchantAppService;
                _communicationLogService = communicationLogService;
                _currentUserDataViewModel = currentUserDataViewModel;
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called before the action executes, after model binding is complete
            /// </summary>
            /// <param name="context">A context for action filters</param>
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                //check whether this filter has been overridden for the Action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(f => f.Scope == FilterScope.Action)
                    .Select(f => f.Filter).OfType<CheckMerchantTerminalAttribute>().FirstOrDefault();

                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                {
                    await next().ConfigureAwait(false);
                    return;
                }

                var securityFailureResponse = new BaseResponse<string>()
                {
                    Data = null,
                    Message = "Security Error.",
                    Success = false,
                    ErrorList = new List<string>()
                };

                //Get Request Body
                var requestBody = context.ActionArguments.Values.LastOrDefault(); // 2 param from every request
                var requestBase = requestBody as IRequestBase;
                if (requestBase is null)
                {
                    context.Result = new BadRequestObjectResult(securityFailureResponse.AddError("Request Base Not Found"));
                    return;
                }

                //var secureHashBase = requestBody as ISecureHashBase;
                //if (secureHashBase is null)
                //{
                //    context.Result = new BadRequestObjectResult(securityFailureResponse.AddError("Secure Hash Base Not Found"));
                //    return;
                //}

                //var requestSource = (RequestSourceEnums) secureHashBase.RequestSource;

                //switch (requestSource)
                //{
                //    case RequestSourceEnums.Portal: //If Portal => Ignore FilterAttribute
                //        await next().ConfigureAwait(false);
                //        return;

                //    case RequestSourceEnums.NoThing://
                //        securityFailureResponse.ErrorList.Add("No Request Source Found.");
                //        context.Result = new BadRequestObjectResult(securityFailureResponse); // Security Error
                //        return;
                //}


                #region old code for bkup

                //Check If Terminal Related To Merchant
                var merchantRequest = new CheckTerminalMerchantRequest()
                {
                    TerminalId = requestBase.TerminalId,
                    MerchantId = requestBase.MerchantId, 
                    MerchantRefId= _currentUserDataViewModel.MerchantRefId.GetValueOrDefault(),
                };
                var merchantTerminalResponse = await _terminalMerchantAppService.IsTerminalMerchantValid(merchantRequest).ConfigureAwait(false);
                if (merchantTerminalResponse is null
                       || merchantTerminalResponse.Success == false
                       || merchantTerminalResponse?.Data == null
                       || merchantTerminalResponse.Data.MerchantRefId == default)
                {
                    securityFailureResponse.ErrorList.Add("There is no Merchant Related to This Terminal");
                    context.Result = new BadRequestObjectResult(securityFailureResponse); // Security Error
                    return;
                }

                if (merchantTerminalResponse.Data.TerminalTypeId is not (int)TerminalTypes.WalletTerminal &&
                    merchantTerminalResponse.Data.TerminalTypeId is not (int)TerminalTypes.WebTerminal)
                {
                    securityFailureResponse.ErrorList.Add("This Terminal is not allowed");
                    context.Result = new BadRequestObjectResult(securityFailureResponse); // Security Error
                    return;
                }

                if (requestBody is ICheckMerchantWithTerminalBase checkMerchantWithTerminalBase)
                {
                    checkMerchantWithTerminalBase.MerchantRefId = merchantTerminalResponse.Data.MerchantRefId;
                    checkMerchantWithTerminalBase.TerminalNodeId = merchantTerminalResponse.Data.TerminalNodeId;
                    checkMerchantWithTerminalBase.TerminalTypeId = merchantTerminalResponse.Data.TerminalTypeId;
                    checkMerchantWithTerminalBase.BankId = merchantTerminalResponse.Data.BankId;
                    checkMerchantWithTerminalBase.AggregatorId = merchantTerminalResponse.Data.AggregatorId;
                    checkMerchantWithTerminalBase.SettAccType = merchantTerminalResponse.Data.SettAccType;
                    checkMerchantWithTerminalBase.MerchantBranchId = merchantTerminalResponse.Data.MerchantBranchId;
                }

                #endregion

                _communicationLogService.SetMerchantRefId(merchantTerminalResponse.Data.MerchantRefId);
                _communicationLogService.SetTerminalNodeId(merchantTerminalResponse.Data.TerminalNodeId);
                _communicationLogService.MPCSSCommunicationLogModel.BankId = merchantTerminalResponse.Data.BankId;

                await next().ConfigureAwait(false);
            }

            #endregion
        }
        #endregion
    }
}
