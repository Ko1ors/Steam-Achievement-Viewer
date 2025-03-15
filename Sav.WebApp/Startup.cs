using ElectronNET.API;
using Microsoft.Extensions.FileProviders;
using Sav.Common.Interfaces;
using Sav.Common.Repositories;
using Sav.Common.Services;
using Serilog;
using System.Xml;

namespace Sav.WebApp
{
    public class Startup
    {
        public async Task ConfigureAsync(string[] args)
        {
            // Configure the HTTP request pipeline.
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseElectron(args);

            builder.Services.AddElectron();

            builder.Services.AddControllers();

            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "/api/{controller=Home}/{action=Index}/{id?}");

            app.MapWhen(context => !context.Request.Path.StartsWithSegments("/api"), builder =>
            {
                builder.UseSpa(spa =>
                {
                    spa.Options.DefaultPageStaticFileOptions = new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                        RequestPath = ""
                    };
                });
            });

            await app.StartAsync();

            // Open the Electron-Window here
            if (HybridSupport.IsElectronActive)
                await Electron.WindowManager.CreateWindowAsync();

            app.WaitForShutdown();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            //Factories
            services.AddScoped(typeof(IServiceFactory<>), typeof(ServiceFactory<>));

            // Services
            services.AddTransient<IClientService<XmlDocument>, XmlClientService>();
            services.AddTransient<ISteamApiClientService, SteamApiClientService>();
            services.AddSingleton<ISteamService, SteamService>();
            services.AddTransient<IGameAchievementsService, GameAchievementsService>();
            services.AddSingleton(typeof(IQueueService<>), typeof(QueueService<>));
            services.AddTransient<IAchievementsWorkerService, AchievementsWorkerService>();

            // Repositories
            services.AddSingleton(typeof(IListRepository<>), typeof(ListRepository<>));
            services.AddTransient(typeof(IEntityRepository<>), typeof(EntityRepository<>));
            services.AddTransient<IUserEntityRepository, UserEntityRepository>();
            services.AddTransient<IGameEntityRepository, GameEntityRepository>();

            // Mapping
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Logging
            services.AddLogging(configure =>
            {
                configure.AddSerilog(dispose: true);
                configure.AddDebug();
                configure.AddConsole();
            });
        }
    }
}
