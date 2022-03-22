using System;
using System.IO;
using System.Reflection;
using FakeDataGenerator.Models.Instant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace FakeDataGenerator
{
    public static class Startup
    {
        public static IHost AppStartup(string[] args)
        {
            IConfiguration configuration = BuildHost(new ConfigurationBuilder());
            var configDataSection = configuration.GetSection("ConfigData");

            var logFilePath = configDataSection.GetValue<string>("LogFilePath");
            var logFileName = configDataSection.GetValue<string>("LogFileName");
            
            // initialize the host
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Adding the DI container for configuration
                    services.Configure<ConfigData>(configDataSection);
                    services.AddScoped(cfg => cfg.GetService<IOptions<ConfigData>>().Value);

                    services.AddScoped<IFakeDataGenerator, FakeDataGenerator>();
                })
                // https://hovermind.com/serilog/logging-to-sink.html
                .UseSerilog(new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .MinimumLevel.Debug()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithProperty("ApplicationName", AppDomain.CurrentDomain.FriendlyName)
                    .Enrich.WithProperty("Version", Assembly.GetEntryAssembly().GetName().Version)
                    .WriteTo.File(
                        path: Path.Combine(logFilePath, $"{logFileName}.{DateTime.Now:yyyyMMdd_HHmm}.txt"),
                        shared: true
                    )
                    .WriteTo.Console(
                        outputTemplate: $"[{{TimeStamp:HH:mm:ss}} {{Level:u3}} {{MachineName}} {{ApplicationName}} {{Version}} ] {{SourceContext}}{{Message:lj}}{{NewLine}}{{Exception}}",
                        theme: AnsiConsoleTheme.Literate
                    )
                    .CreateLogger())
                .Build();
            
            return host;
        }

        private static IConfiguration BuildHost(ConfigurationBuilder configurationBuilder)
        {
            string env = Environment.GetEnvironmentVariable("ASPNETCORE_VARIABLE") ?? Environments.Production;
            
            return configurationBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env}.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}