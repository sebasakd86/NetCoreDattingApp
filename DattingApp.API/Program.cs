using System;
using DattingApp.API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DattingApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // To seed the data.
            var host = CreateHostBuilder(args).Build();// .Run();

            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<DataContext>();
                    context.Database.Migrate();
                    Seed.SeedUsers(context);
                }
                catch (System.Exception ex)
                {
                    var log = services.GetRequiredService<ILogger<Program>>();
                    Console.WriteLine($"Error while seeding data --> {ex.Message}");
                    log.LogError(ex, "Error while seeding data");
                    
                }
            }
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
