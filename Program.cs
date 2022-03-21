using System;
using System.Linq;
using System.Threading.Tasks;
using FakeDataGenerator.Models.Instant;
using Microsoft.Extensions.DependencyInjection;

namespace FakeDataGenerator
{
    class Program
    {
        // default value is going to be 100
        private static int _amountOfGeneratedData = 100;
        private static ConfigData _config;
        private static IFakeDataGenerator _fakeDataGenerator;
            
        static async Task Main(string[] args)
        {
            var host = Startup.AppStartup(args);
            _config = host.Services.GetService<ConfigData>();
            _fakeDataGenerator = host.Services.GetService<IFakeDataGenerator>();

            if (args.Length > 0 && args[0].All(char.IsDigit))
                Int32.TryParse(args[0], out _amountOfGeneratedData);

            await StartAsync(_amountOfGeneratedData);
        }

        private static async Task StartAsync(int amountOfGeneratedData)
        {
            try
            {
                await _fakeDataGenerator.GeneratedDataAsync(amountOfGeneratedData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Issue occur during processing. {ex.Message}");
            }
        }
    }
}