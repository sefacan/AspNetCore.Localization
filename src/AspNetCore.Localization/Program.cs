using AspNetCore.Localization.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Localization
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            //apply database migration
            using (var scope = host.Services.CreateScope())
            using (var context = scope.ServiceProvider.GetRequiredService<AppDataContext>())
            {
                var pendingMigration = await context.Database.GetPendingMigrationsAsync();
                if (pendingMigration.Any())
                    await context.Database.MigrateAsync();
            }

            await host.RunAsync().ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
