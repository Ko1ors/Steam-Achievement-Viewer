using Microsoft.Extensions.DependencyInjection;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Pages;
using SteamAchievementViewer.Services;
using SteamAchievementViewer.ViewModels;
using System.Windows;
using System.Xml;

namespace SteamAchievementViewer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IClientService<XmlDocument>, XmlClientService>();
            services.AddSingleton<ISteamService, SteamService>();
            services.AddTransient<IGameAchievementsService, GameAchievementsService>();

            // ViewModels
            services.AddScoped<MainWindowViewModel>();

            // Pages
            services.AddTransient<AuthPage>();
            services.AddTransient<CloseAchievements>();
            services.AddTransient<CloseAllAchievements>();
            services.AddTransient<LastAchievedPage>();
            services.AddTransient<MainPageInfo>();
            services.AddTransient<RareAchievements>();
            services.AddTransient<SettingsPage>();

            // Windows
            services.AddSingleton<MainWindow>();
        }

        private void ConfigureNavigation()
        {
            var navigationService = ServiceProvider.GetService<INavigationService>();
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(MainPageInfo), Title = SteamAchievementViewer.Properties.Resources.MainPage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(AuthPage), Title = SteamAchievementViewer.Properties.Resources.AuthPage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(LastAchievedPage), Title = SteamAchievementViewer.Properties.Resources.LastAchievedPage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(CloseAchievements), Title = SteamAchievementViewer.Properties.Resources.ClosestPage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(CloseAllAchievements), Title = SteamAchievementViewer.Properties.Resources.ClosestAllPage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(RareAchievements), Title = SteamAchievementViewer.Properties.Resources.RarestPage });
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ConfigureNavigation();
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();

            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
        }
    }
}
