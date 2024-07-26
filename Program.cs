using System;
using System.IO;
using ActivityLookUp;
using ActivityLookUp.Npsql;
using ActivityLookUp.Processes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.SetBasePath(Directory.GetCurrentDirectory())
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                // Register configuration and services
                IConfiguration configuration = context.Configuration;

                var appSettings = new AppSettings();
                configuration.Bind("AppSettings", appSettings);
                services.AddSingleton(appSettings);

                services.AddSingleton<WindowsProcess>();
                services.AddSingleton<NpsqlRepository>();

                services.AddHostedService<Worker>();
            });
}
