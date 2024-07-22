using System.Windows.Media.Imaging;

namespace SteamAchievementViewer
{
    public interface IImageService
    {
        BitmapImage GetImage(string url, bool useCache = true);
    }
}
