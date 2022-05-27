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
        private Frame _frame;

        public NavigationService()
        {
            _pageElements = new List<NavigationPageElement>();
        }

        public void AddPageElement(NavigationPageElement element)
        {
            _pageElements.Add(element);
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
            _pageElements.Where(npe => npe.Selected is true && npe != element).ToList().ForEach(npe => npe.Selected = false);
            element.Selected = true;
            _frame.Content = App.ServiceProvider.GetService(element.Type);
        }

        public void SetNavigationFrame(Frame frame)
        {
            _frame = frame;
        }
    }
}
