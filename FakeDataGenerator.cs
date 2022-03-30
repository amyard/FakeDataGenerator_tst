﻿using System;
using System.IO;
using System.Threading.Tasks;
using FakeDataGenerator.Models.General;
using Serilog;

namespace FakeDataGenerator
{
    public interface IFakeDataGenerator
    {
        Task GeneratedDataAsync(int amountOfGeneratedData);
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

        public async Task GeneratedDataAsync(int amountOfGeneratedData)
        {
            _logger.Information("Start processing.");
            
            string fullPath = GenerateFullPath(amountOfGeneratedData);

            var instantData = _generateFakeDataEntities.GenerateInstantEntity(amountOfGeneratedData);
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