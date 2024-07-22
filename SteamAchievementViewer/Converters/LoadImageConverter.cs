using Microsoft.Extensions.DependencyInjection;
using Sav.Common.Interfaces;
using System;
using System.Globalization;
using System.Windows.Data;

namespace SteamAchievementViewer.Converters
{
    public class LoadImageConverter : IValueConverter
    {
        private static IImageService _imageService;

        private static IImageService ImageService => _imageService ??= App.ServiceProvider.GetService<IImageService>();


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            return ImageService.GetImage(value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
