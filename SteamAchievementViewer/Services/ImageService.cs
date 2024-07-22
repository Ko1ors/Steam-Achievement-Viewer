using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace SteamAchievementViewer.Services
{
    public class ImageService : IImageService
    {
        private static readonly Dictionary<string, BitmapImage> _cache = new Dictionary<string, BitmapImage>();

        public BitmapImage GetImage(string url, bool useCache = true)
        {
            if (useCache && _cache.ContainsKey(url))
            {
                return _cache[url];
            }
            else
            {
                BitmapImage image = new BitmapImage(new Uri(url));
                image.CacheOption = BitmapCacheOption.OnLoad;
                _cache[url] = image;
                return image;
            }
        }
    }
}
