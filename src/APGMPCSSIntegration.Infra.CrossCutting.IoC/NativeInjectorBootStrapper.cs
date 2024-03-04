using APG.MessageQueue.Mpcss.Interfaces;
using APG.MessageQueue.Mpcss.Services;
using APGExecutions.IAL.Internal.Interfaces.APGFundamentals;
using APGDigitalIntegration.Application.Services;
using APGDigitalIntegration.Domain.Events;
using APGMPCSSIntegration.Application.Services;
using APGMPCSSIntegration.DomainHelper.Middlewares;
using APGMPCSSIntegration.IAL.Internal.Services.APGFundamentals;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using APGDigitalIntegration.IAL.External.Mpcss.Communicators;
using APGDigitalIntegration.Application.Interfaces;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamentals;
using APGExecutions.IAL.Internal.Services.APGFundamentals;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGTransaction;
using APGDigitalIntegration.IAL.Internal.Services.APGTransaction;
using APGDigitalIntegration.Application.InternalResponsesValidators.Interfaces;
using APGDigitalIntegration.Application.InternalResponsesValidators.Validators;
using APGDigitalIntegration.BackgroundJobs.Helpers;
using APGDigitalIntegration.Cache.Interfaces;
using APGDigitalIntegration.Cache.Services;
using APGDigitalIntegration.DomainHelper.Interfaces;
using APGDigitalIntegration.DomainHelper.Services;
using APGDigitalIntegration.Common.Observers;
using APGDigitalIntegration.Common.CommonServices;
using APGDigitalIntegration.Domain.Interfaces;
using APGDigitalIntegration.IAL.External.Hosts.CBOHosts;
using APGDigitalIntegration.IAL.External.Interfaces;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Operational;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Inward;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.CBOHostAdapters.Transactional.Outward;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGFundamental;
using APGDigitalIntegration.IAL.Internal.Services.APGFundamental;
using APGDigitalIntegration.IAL.External.Mpcss.Hosts.SimulateAdapter;
using APGDigitalIntegration.IAL.External.Mpcss.HostsFactories;
using APGDigitalIntegration.IAL.External.Mpcss.Interfaces;
using APGDigitalIntegration.IAL.Internal.Communicator;
using APGFundamentals.Application.Helper;
using APGMPCSSIntegration.Constant;
using APGMPCSSIntegration.IAL.External.Hosts.CBOHosts;
using APGDigitalIntegration.Infra.Data.Repository;
using APGDigitalIntegration.Application.ViewModels;
using Microsoft.AspNetCore.Http;
using APGDigitalIntegration.IAL.Internal.Interfaces.APGMembership;
using APGMPCSSIntegration.IAL.Internal.Services.APGMembership;
using APGDigitalIntegration.IAL.External.Mpcss.Helper;

namespace APGDigitalIntegration.Infra.CrossCutting.IoC
{
    public static class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Domain Bus (Mediator)
            // services.AddScoped<IMediatorHandler, InMemoryBus>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IManualResetProvider, ManualResetProvider>();
            services.AddSingleton<ISystemTokenService, SystemTokenService>();

            // Application
            services.AddScoped<IDigitalOperationAppService, DigitalOperationAppService>();
            services.AddScoped<IDigitalTransactionAppService, DigitalTransactionAppService>();
            services.AddScoped<IBaseDigitalIntegration, BaseDigitalIntegration>();
            services.AddScoped<ISimulatedReceiver, SimulatedReceiver>();
            services.AddScoped<ISimulationAppService, SimulationAppService>();
            services.AddScoped<ICommonTransactionalAppService, CommonTransactionalAppService>();
            services.AddScoped<IQRCodeAppService, QRCodeAppService>();
            services.AddScoped<IMerchantMPCSSOperationAppService, MerchantMPCSSOperationAppService>();
            services.AddScoped<IMerchantMPCSSOperationAppService, MerchantMPCSSOperationAppService>();
            services.AddScoped<IMPCSSCommunicationLogService, MPCSSCommunicationLogService>();
            services.AddScoped<IHealthCheckAppService, HealthCheckAppService>();
            services.AddScoped<IAuthenticateService,AuthenticateService>();


            services.AddScoped<ISimulateHostAdapter, SimulateHostAdapter>();
           
            services.AddScoped<IDigitalTransactionReadAppService, DigitalTransactionReadReadAppService>();

            // Domain - Events
            services.AddScoped<INotificationHandler<CustomerRegisteredEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerUpdatedEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerRemovedEvent>, CustomerEventHandler>();
            
            services.AddScoped<IAmountConverter, AmountConverter>();

            // Domain - Commands
            // services.AddScoped<IRequestHandler<RegisterNewCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            // services.AddScoped<IRequestHandler<UpdateCustomerCommand, ValidationResult>, CustomerCommandHandler>();
            // services.AddScoped<IRequestHandler<RemoveCustomerCommand, ValidationResult>, CustomerCommandHandler>();


            services.AddSingleton<TransactionTypesCacheService>();
            
            services.AddScoped<ILoggingService, LoggingService>();
            services.AddScoped<IDateTimeProvider, DatetimeOffsetProvider>();
            services.AddScoped<IMPCSSMessageBuilder, MPCSSMessageBuilder>();

            // Localization
            services.AddLocalization();
            services.AddSingleton<LocalizationMiddleware>();
            services.AddDistributedMemoryCache();

            //IAL Internal
            services.AddScoped<IApiCaller, ApiCaller>();
            services.AddScoped<IMerchantOrderApiService, MerchantOrderApiService>();
            services.AddScoped<ITransactionApiService, TransactionApiService>();
            services.AddHttpClient();

            //IAL Internal Services
            services.AddScoped<IConfParamHelperService, ConfParamHelperService>();
            services.AddScoped<IMerchantAppService, MerchantAppService>();
            services.AddScoped<ITerminalMerchantAppService, TerminalMerchantAppService>();
            services.AddScoped<IShadowBalanceAppService, ShadowBalanceAppService>();
            services.AddScoped<ITransactionHelper, TransactionHelper>();
            services.AddScoped<ICurrencyApiService, CurrencyApiService>();
            services.AddScoped<IBankApiService, BankApiService>();
            services.AddScoped<IMembershipService, MembershipService>();


            //IAL External
            services.AddScoped<IMpcssCommunicator, MpcssCommunicator>();

            // Cache
            services.AddSingleton<ICacheService, RedisCacheService>();

            // Factories
            services.AddScoped (typeof(IBaseHostFactory<>), typeof(BaseHostFactory<>));

            // Validators
            services.AddScoped<ICheckShadowBalanceLimitResponseValidator, CheckShadowBalanceLimitResponseValidator>();
            services.AddScoped<IMerchantRefIdByMerchantIdResponseValidator, MerchantRefIdByMerchantIdResponseValidator>();
            services.AddScoped<IAuthenticateBaseValidator, AuthenticateBaseValidator>();
            services.AddScoped<ResponseMessageHandler>();
            
            services.AddScoped<AccountRegistrationHostAdapter>();
            services.AddScoped<AccountVerificationHostAdapter>();
            services.AddScoped<RecordRegistrationHostAdapter>();
            services.AddScoped<RecordVerificationHostAdapter>();
            
            services.AddScoped<PaymentCreditInwardHostAdapter>();
            services.AddScoped<PaymentDebitInwardHostAdapter>();
            services.AddScoped<PaymentReturnInwardHostAdapter>();
            
            services.AddScoped<PaymentCreditOutwardHostAdapter>();
            services.AddScoped<PaymentDebitOutwardHostAdapter>();
            services.AddScoped<PaymentReturnOutwardHostAdapter>();
            services.AddScoped<PaymentEnquiryHostAdapter>();
            services.AddScoped<PaymentStatusReportHostAdapter>();
            
            
            // Data
            services.AddScoped<IDigitalTransactionRepository, DigitalTransactionRepository>();
            services.AddScoped<IDigitalTransactionEnquiryRepository, DigitalTransactionEnquiryRepository>();
            services.AddScoped<IMerchantMPCSSTransactionRequestsRepository, MerchantMPCSSTransactionRequestsRepository>();
            services.AddScoped<CurrentUserDataViewModel>();
        }
    }
}