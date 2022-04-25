using System.Text.Json;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace FakeDataGenerator.Models.CsvMapping
{
    public class JsonConverter<T> : DefaultTypeConverter
    {
        private static readonly JsonSerializerOptions _jsonOptions = new ()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
        {
            return JsonSerializer.Deserialize<T>(text);
        }

        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            return JsonSerializer.Serialize(value, _jsonOptions);
        }

    }
}