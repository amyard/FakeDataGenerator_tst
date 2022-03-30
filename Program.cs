using System;
using System.Linq;
using System.Threading.Tasks;
using FakeDataGenerator.Models.General;
using Microsoft.Extensions.DependencyInjection;

namespace FakeDataGenerator
{
    class Program
    {
        private static IFakeDataGenerator _fakeDataGenerator;
            
        static async Task Main(string[] args)
        {
            var host = Startup.AppStartup(args);
            _fakeDataGenerator = host.Services.GetService<IFakeDataGenerator>();

            ArgumentEntity argumentEntity = new ArgumentEntity(args);
            
            await StartAsync(argumentEntity);
        }

        private static async Task StartAsync(ArgumentEntity argumentEntity)
        {
            try
            {
                await _fakeDataGenerator.GeneratedDataAsync(argumentEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Issue occur during processing. {ex.Message}");
            }
        }
    }
}