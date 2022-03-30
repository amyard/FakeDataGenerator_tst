using System;
using System.IO;
using System.Threading.Tasks;
using FakeDataGenerator.Enums;
using FakeDataGenerator.Models.General;
using Serilog;

namespace FakeDataGenerator
{
    public interface IFakeDataGenerator
    {
        Task GeneratedDataAsync(ArgumentEntity argumentEntity);
    }
    
    public class FakeDataGenerator: IFakeDataGenerator
    {
        private readonly string _generatedDataPath;
        private readonly ILogger _logger;
        private readonly IFileHandler _fileHandler;
        private readonly IGenerateFakeDataEntities _generateFakeDataEntities;

        public FakeDataGenerator(ConfigData config, ILogger logger, IFileHandler fileHandler, 
            IGenerateFakeDataEntities generateFakeDataEntities)
        {
            _generatedDataPath = config.GeneratedDataPath;
            _logger = logger;
            _fileHandler = fileHandler;
            _generateFakeDataEntities = generateFakeDataEntities;
            PrepareEnvironment();
        }

        public async Task GeneratedDataAsync(ArgumentEntity argumentEntity)
        {
            _logger.Information("Start processing.");
            
            string fullPath = GenerateFullPath(argumentEntity.AmountOfGeneratedData);

            var instantData = _generateFakeDataEntities.GenerateInstantEntity(argumentEntity.AmountOfGeneratedData);

            // var data = argumentEntity.EntityNameForMapping switch
            // {
            //     ClassNameForMapping.Instant => _generateFakeDataEntities.GenerateInstantEntity(argumentEntity
            //         .AmountOfGeneratedData),
            //     ClassNameForMapping.TableUser => _generateFakeDataEntities.GenerateTableUserEntity(argumentEntity
            //         .AmountOfGeneratedData),
            //     _ => _generateFakeDataEntities.GenerateInstantEntity(argumentEntity.AmountOfGeneratedData)
            // };
            
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