using APGDigitalIntegration.IAL.External.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace APGDigitalIntegration.IAL.External.Mpcss.HostsFactories
{
    public class BaseHostFactory<T> : IBaseHostFactory<T>
    {
        private readonly IServiceProvider _serviceProvider;

        public BaseHostFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public T CreateHost(params object[] args)
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
