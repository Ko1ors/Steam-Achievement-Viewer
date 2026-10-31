using Sav.Common.Models;

namespace Sav.Common.Interfaces
{
    public interface ISettingsService
    {
        UserSettings Get();

        void Save(UserSettings settings);
    }
}
