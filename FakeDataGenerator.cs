using System;
using System.IO;
using System.Threading.Tasks;
using FakeDataGenerator.Enums;
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
            
            var process = new DataProcessFactory().GetProcess(argumentEntity);
            var instantData = process.GenerateFakeDataEntities(argumentEntity.AmountOfGeneratedData);

            string fullPath = GenerateFullPath(argumentEntity);
            
            switch (argumentEntity.SaveAsExtension)
            {
                case FileExtensions.Json:
                    await _fileHandler.SaveAsJsonFileAsync(instantData, fullPath);
                    break;
                case FileExtensions.Csv:
                    await _fileHandler.SaveAsCsvFileAsync(instantData, fullPath, argumentEntity);
                    break;
            }
            
            _logger.Information("The process completed.");  
        }

        private string GenerateFullPath(ArgumentOptions argumentOptions)
        {
            return Path.Combine(_generatedDataPath,
                $"Date_{DateTime.Now:dd_MM_yyyy_hh_ss}-iteration_{argumentOptions.AmountOfGeneratedData}.{argumentOptions.SaveAsExtension.ToString().ToLower()}");
        }

        private void PrepareEnvironment()
        {
            if (!Directory.Exists(_generatedDataPath))
                Directory.CreateDirectory(_generatedDataPath);
        }
    }
}