using Sav.Common.Logs;
using SteamAchievementViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace SteamAchievementViewer.Services
{
    public class NavigationService : INavigationService
    {
        private List<NavigationPageElement> _pageElements;

        public event AvailabilityNotify AvailabilityChanged;
        public event NavigationNotify NavigationChanged;

        public NavigationService()
        {
            _pageElements = new List<NavigationPageElement>();
        }

        public void AddPageElement(NavigationPageElement element)
        {
            _pageElements.Add(element);
        }

        public void ChangeAvailability(bool isAvailable)
        {
            Log.Logger.Information("Availability changed to {IsAvailable}", isAvailable);
            AvailabilityChanged?.Invoke(isAvailable);
        }

        public IEnumerable<NavigationPageElement> GetPageElements()
        {
            return _pageElements;
        }

        public void NavigateTo(Type pageType)
        {
            var pageElement = _pageElements.FirstOrDefault(p => p.GetType() == pageType);
            if (pageElement != null)
                NavigateTo(pageElement);
        }

        public void NavigateTo(NavigationPageElement element)
        {
            Log.Logger.Information("Navigating to {PageType}", element.Type.Name);
            _pageElements.Where(npe => npe.Selected is true && npe != element).ToList().ForEach(npe => npe.Selected = false);
            element.Selected = true;
            NavigationChanged?.Invoke(App.ServiceProvider.GetService(element.Type) as Page);
        }
    }
}
