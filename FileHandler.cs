using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Serilog;

namespace FakeDataGenerator
{
    public interface IFileHandler
    {
        Task SaveAsJsonFileAsync<T>(List<T> data, string fullPath);
    }
    
    public class FileHandler: IFileHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        private readonly ILogger _logger;

        public FileHandler(ILogger logger)
        {
            _logger = logger;
        }

        public async Task SaveAsJsonFileAsync<T>(List<T> data, string fullPath)
        {
            var jsonString = JsonSerializer.Serialize(data, _jsonOptions);

            try
            {
                await using (TextWriter writer = new StreamWriter(fullPath, append: false))
                {
                    await writer.WriteLineAsync(jsonString);
                    await writer.FlushAsync();
                }

                // ReSharper disable once SuggestVarOrType_SimpleTypes
                FileInfo fileInfo = new FileInfo(fullPath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    _logger.Information("The file was generated with {0} entities. File size is {1} Mb",
                        data.Count,
                        ConvertBytesToMegaBytes(fileInfo.Length));
                }
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
    }
}