using ElectronNET.API;

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
            app.UseAuthorization();


            app.MapControllerRoute(
                name: "default",
                pattern: "/api/{controller=Home}/{action=Index}/{id?}");

            await app.StartAsync();

            // Open the Electron-Window here
            if (HybridSupport.IsElectronActive)
                await Electron.WindowManager.CreateWindowAsync();

            app.WaitForShutdown();
        }
    }
}
