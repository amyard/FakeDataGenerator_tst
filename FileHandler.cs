using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FakeDataGenerator.Enums;
using FakeDataGenerator.Models.CsvMapping;
using FakeDataGenerator.Models.General;
using Serilog;

namespace FakeDataGenerator
{
    public interface IFileHandler
    {
        Task SaveAsJsonFileAsync<T>(List<T> data, string fullPath);
        Task SaveAsCsvFileAsync<T>(List<T> data, string fullPath, ArgumentOptions argumentOptions);
    }
    
    public class FileHandler: IFileHandler
    {
        private readonly ILogger _logger;

        public FileHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task SaveAsJsonFileAsync<T>(List<T> data, string fullPath)
        {
            JsonSerializerOptions _jsonOptions = new ()
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        
            var jsonString = JsonSerializer.Serialize(data, _jsonOptions);

            try
            {
                await using (TextWriter writer = new StreamWriter(fullPath, append: false))
                {
                    await writer.WriteLineAsync(jsonString);
                    await writer.FlushAsync();

                    SuccessProcessingMessage(fullPath, data.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured by saving the data: {0}", ex.Message);
            }
        }

        public async Task SaveAsCsvFileAsync<T>(List<T> data, string fullPath, ArgumentOptions argumentOptions)
        {
            try
            {
                CsvConfiguration _csvOptions = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = true,
                    Delimiter = ";"
                };

                await using var writer = new StreamWriter(fullPath);
                await using var csvWriter = new CsvWriter(writer, _csvOptions);
                
                switch (argumentOptions.EntityNameForMapping)
                {
                    case ClassNameForMapping.Instant:
                        csvWriter.Context.RegisterClassMap<InstantClassMap>();
                        break;
                    case ClassNameForMapping.TableUser:
                        csvWriter.Context.RegisterClassMap<TableUserClassMap>();
                        break;
                }
                
                csvWriter.WriteHeader<T>();
                await csvWriter.WriteRecordsAsync(data);
                await writer.FlushAsync();
                
                SuccessProcessingMessage(fullPath, data.Count);
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured by saving the data: {0}", ex.Message);
            }
        }

        private double ConvertBytesToMegaBytes(long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f, 4);
        }

        private void SuccessProcessingMessage(string fullPath, int amountOfGeneratedData)
        {
            FileInfo fileInfo = new FileInfo(fullPath);
            if (fileInfo.Exists && fileInfo.Length > 0)
            {
                _logger.Information("The file was generated with {0} entities. File size is {1} Mb",
                    amountOfGeneratedData,
                    ConvertBytesToMegaBytes(fileInfo.Length));
            }
        }
    }
}