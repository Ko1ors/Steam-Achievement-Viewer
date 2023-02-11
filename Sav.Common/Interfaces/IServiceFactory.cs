namespace Sav.Common.Interfaces
{
    public interface IServiceFactory<out T> where T : class
    {
        T Create();
    }
}
