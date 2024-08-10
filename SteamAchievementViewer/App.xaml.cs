using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sav.Common.Interfaces;
using Sav.Common.Logs;
using Sav.Common.Repositories;
using Sav.Common.Services;
using Sav.Infrastructure;
using Serilog;
using SteamAchievementViewer.Mapping;
using SteamAchievementViewer.Models;
using SteamAchievementViewer.Pages;
using SteamAchievementViewer.Services;
using SteamAchievementViewer.ViewModels;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Log = Sav.Common.Logs.Log;

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
            Serilog.Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/log-.log", rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {SourceContext} {MemberName}() | {Message} {Exception}{NewLine}")
                .CreateLogger();

            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //Factories
            services.AddScoped(typeof(IServiceFactory<>), typeof(ServiceFactory<>));

            // Services
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddTransient<IClientService<XmlDocument>, XmlClientService>();
            services.AddTransient<ISteamApiClientService, SteamApiClientService>();
            services.AddSingleton<ISteamService, SteamService>();
            services.AddTransient<IGameAchievementsService, GameAchievementsService>();
            services.AddSingleton(typeof(IQueueService<>), typeof(QueueService<>));
            services.AddTransient<IAchievementsWorkerService, AchievementsWorkerService>();
            services.AddSingleton<IImageService, ImageService>();

            // Repositories
            services.AddSingleton(typeof(IListRepository<>), typeof(ListRepository<>));
            services.AddTransient(typeof(IEntityRepository<>), typeof(EntityRepository<>));
            services.AddTransient<IUserEntityRepository, UserEntityRepository>();
            services.AddTransient<IGameEntityRepository, GameEntityRepository>();

            // Mapping
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // ViewModels
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<AuthPageViewModel>();
            services.AddScoped<EasiestGamesToCompleteViewModel>();
            services.AddScoped<CompletedGamesViewModel>();
            services.AddScoped<MainInfoViewModel>();
            services.AddScoped<SettingsPageViewModel>();

            // Pages
            services.AddTransient<AuthPage>();
            services.AddTransient<CloseAchievements>();
            services.AddTransient<CloseAllAchievements>();
            services.AddTransient<LastAchievedPage>();
            services.AddTransient<MainPageInfo>();
            services.AddTransient<RareAchievements>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<EasiestGamesToCompletePage>();
            services.AddTransient<CompletedGamesPage>();

            // Windows
            services.AddSingleton<MainWindow>();

            // Logging
            services.AddLogging(configure =>
            {
                configure.AddSerilog(dispose: true);
                configure.AddDebug();
                configure.AddConsole();
            });
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
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(EasiestGamesToCompletePage), Title = SteamAchievementViewer.Properties.Resources.EasiestGamesToCompletePage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(CompletedGamesPage), Title = SteamAchievementViewer.Properties.Resources.CompletedGamesPage });
            navigationService.AddPageElement(new NavigationPageElement { Type = typeof(SettingsPage), Title = SteamAchievementViewer.Properties.Resources.SettingsPage });

        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using (var context = new SteamContext())
            {
                context.Database.Migrate();
            }

            var config = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); }).CreateMapper();
            config.ConfigurationProvider.AssertConfigurationIsValid();


            ConfigureNavigation();
            var workerFactory = ServiceProvider.GetService<IServiceFactory<IAchievementsWorkerService>>();
            // TODO: move worker count to the config
            for (int i = 0; i < 5; i++)
            {
                Task.Run(() => workerFactory.Create().StartAsync(new CancellationTokenSource().Token));
            }


            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();

            //System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("en");
        }


        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Logger.Fatal(e.ExceptionObject as Exception, "Unhandled Exception: ");
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Logger.Fatal(e.Exception, "UI Thread Unhandled Exception: ");
            Current.Shutdown();
            e.Handled = true;
        }
    }
}
