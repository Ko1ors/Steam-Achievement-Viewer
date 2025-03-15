using System.Threading.Tasks;

namespace Sav.Common.Services
{
    public interface IClientService<T> where T : class
    {
        Task<T> SendGetRequest(string requestUrl);
    }
}
