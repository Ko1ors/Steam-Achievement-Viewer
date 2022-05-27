using SteamAchievementViewer.Models;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace SteamAchievementViewer.Services
{
    public interface INavigationService
    {
        void SetNavigationFrame(Frame frame);

        void AddPageElement(NavigationPageElement element);

        IEnumerable<NavigationPageElement> GetPageElements();

        void NavigateTo(Type pageType);

        void NavigateTo(NavigationPageElement element);
    }
}
