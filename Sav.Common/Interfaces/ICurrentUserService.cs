namespace Sav.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? SteamId { get; }

        bool IsAuthenticated { get; }

        void SetCurrentUser(string steamId);
    }
}
