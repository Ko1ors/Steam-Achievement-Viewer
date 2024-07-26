using Microsoft.Extensions.DependencyInjection;
using Sav.Common.Interfaces;
using Sav.Common.Logs;

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
            Log.Logger.Information("Creating service {Service}", typeof(T).Name);
            return _serviceProvider.GetRequiredService<T>();
        }
    }
}
