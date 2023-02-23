using Microsoft.Extensions.DependencyInjection;
using Sav.Common.Interfaces;

namespace Sav.Common.Services
{
    public class ServiceFactory<T> : IServiceFactory<T> where T : class
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create()
        {
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
