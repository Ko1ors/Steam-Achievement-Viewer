using Sav.Common.Logs;
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
            Log.Logger.Information("Requesting image {Url}, {UseCache}", url, useCache);
            if (useCache && _cache.ContainsKey(url))
            {
                Log.Logger.Information("Image {Url} found in cache", url);
                return _cache[url];
            }
            else
            {
                Log.Logger.Information("Image {Url} not found in cache or useCache is false, fetching", url);
                BitmapImage image = new BitmapImage(new Uri(url));
                image.CacheOption = BitmapCacheOption.OnLoad;
                _cache[url] = image;
                return image;
            }
        }
    }
}
