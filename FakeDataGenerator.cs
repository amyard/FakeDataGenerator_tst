using System;
using System.IO;
using System.Threading.Tasks;
using FakeDataGenerator.Models.General;
using FakeDataGenerator.Processes;
using Serilog;

namespace FakeDataGenerator
{
    public interface IFakeDataGenerator
    {
        Task GeneratedDataAsync(ArgumentOptions argumentEntity);
    }
    
    public class FakeDataGenerator: IFakeDataGenerator
    {
        private readonly string _generatedDataPath;
        private readonly ILogger _logger;
        private readonly IFileHandler _fileHandler;

        public FakeDataGenerator(ConfigData config, 
            ILogger logger, 
            IFileHandler fileHandler)
        {
            _generatedDataPath = config.GeneratedDataPath;
            _logger = logger;
            _fileHandler = fileHandler;
            PrepareEnvironment();
        }

        public async Task GeneratedDataAsync(ArgumentOptions argumentEntity)
        {
            _logger.Information("Start processing.");
            
            string fullPath = GenerateFullPath(argumentEntity.AmountOfGeneratedData);
            
            var process = new DataProcessFactory().GetProcess(argumentEntity);
            var instantData = process.GenerateFakeDataEntities(argumentEntity.AmountOfGeneratedData);

            await _fileHandler.SaveAsJsonFileAsync(instantData, fullPath);
            
            _logger.Information("The process completed.");  
        }

        private string GenerateFullPath(int amountOfGeneratedData)
        {
            return Path.Combine(_generatedDataPath,
                $"Date_{DateTime.Now:dd_MM_yyyy_hh_ss}-iteration_{amountOfGeneratedData}.json");
        }

        private void PrepareEnvironment()
        {
            if (!Directory.Exists(_generatedDataPath))
                Directory.CreateDirectory(_generatedDataPath);
        }
    }
}