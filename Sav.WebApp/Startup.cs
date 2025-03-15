using ElectronNET.API;
using Microsoft.Extensions.FileProviders;

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
    }
}
