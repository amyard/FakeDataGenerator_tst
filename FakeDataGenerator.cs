using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using FakeDataGenerator.Models.Instant;
using Serilog;

namespace FakeDataGenerator
{
    public interface IFakeDataGenerator
    {
        Task GeneratedDataAsync(int amountOfGeneratedData);
    }
    
    public class FakeDataGenerator: IFakeDataGenerator
    {
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        
        // external log id for testing purpose
        private static string[] _externalLocationIds = new string[] {"123", "321"};
        private readonly string _generatedDataPath;
        private readonly ILogger _logger;

        public FakeDataGenerator(ConfigData config, ILogger logger)
        {
            _generatedDataPath = config.GeneratedDataPath;
            _logger = logger;
            PrepareEnvironment();
        }


        public async Task GeneratedDataAsync(int amountOfGeneratedData)
        {
            _logger.Information("Start processing.");

            Random random = new Random();
            string externalLocId = _externalLocationIds[random.Next(_externalLocationIds.Length)];
            string fullPath = Path.Combine(_generatedDataPath,
                $"Date_{DateTime.Now.ToString("dd_MM_yyyy_hh_ss")}-iteration_{amountOfGeneratedData}-locid_{externalLocId}.json");

            var jobInfo = new Faker<JobInfo>()
                .RuleFor(o => o.HourlyRate, f => f.Finance.Amount(0, 20).OrNull(f, .5f))
                .RuleFor(o => o.AnnualSalary, (f, o) =>
                {
                    return o.HourlyRate == null ? o.AnnualSalary = f.Finance.Amount(0, 10000) : null;
                })
                .RuleFor(o => o.Code, f => f.Random.AlphaNumeric(random.Next(10, 20)).OrNull(f, .2f))
                .RuleFor(o => o.IsHourly, f => f.Random.Bool())
                .RuleFor(o => o.IsDefault, f => f.Random.Bool());

            var terminated = new Faker<Terminated>()
                .RuleFor(t => t.IsTerminated, f => f.Random.Bool())
                .Rules((f, t) =>
                {
                    if (!t.IsTerminated) t.TerminatedData = f.Date.Recent(30).ToString("yyyyy-MM-dd");
                    else t.TerminatedData = null;
                });

            var employeeInfo = new Faker<EmployeeInfo>()
                .RuleFor(o => o.EmployeeId, f => f.Random.AlphaNumeric(random.Next(18, 99)))
                .RuleFor(o => o.FirstName, f => f.Name.FirstName())
                .RuleFor(o => o.LastName, f => f.Name.LastName())
                .RuleFor(o => o.BirthDate, f => f.Date.Past(70, DateTime.Now.AddYears(20)).ToString("yyyy-MM-dd"))
                .RuleFor(o => o.BlockInstantPay, f => f.Random.Bool())
                .RuleFor(o => o.Terminated, (f, o) =>
                {
                    var terminatedGenerated = terminated.Generate();
                    return !string.IsNullOrWhiteSpace(terminatedGenerated.TerminatedData)
                        ? terminatedGenerated.TerminatedData
                        : $"{terminatedGenerated.IsTerminated}";
                })
                .RuleFor(o => o.PaidOnInstant, f => f.Random.Bool());

            var instantEntity = new Faker<InstantPaycardInfo>()
                .RuleFor(o => o.EnternalLocationId, f => externalLocId)
                .RuleFor(o => o.JobInfo, f => jobInfo.Generate(random.Next(1, 5)).ToArray())
                .RuleFor(o => o.EmployeeInfo, f => employeeInfo.Generate());

            var instantData = instantEntity.Generate(amountOfGeneratedData);
            
            instantData.ForEach(entity =>
            {
                foreach (var job in entity.JobInfo)
                {
                    job.Name = $"{entity.EmployeeInfo.LastName} {entity.EmployeeInfo.FirstName}";
                }
            });

            var jsonString = JsonSerializer.Serialize(instantData, _jsonOptions);

            try
            {
                using (TextWriter writer = new StreamWriter(fullPath, append: false))
                {
                    await writer.WriteLineAsync(jsonString);
                    await writer.FlushAsync();
                }

                FileInfo fileInfo = new FileInfo(fullPath);
                if (fileInfo.Exists && fileInfo.Length > 0)
                {
                    _logger.Information("The file was generated with {0} entities. File size is {1} Mb",
                        amountOfGeneratedData,
                        ConvertBytesToMegaBytes(fileInfo.Length));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured by saving the data: {0}", ex.Message);
            }
            finally
            {
                _logger.Information("The process completed.");   
            }
        }
        
        private void PrepareEnvironment()
        {
            if (!Directory.Exists(_generatedDataPath))
                Directory.CreateDirectory(_generatedDataPath);
        }

        private double ConvertBytesToMegaBytes(long bytes)
        {
            return Math.Round((bytes / 1024f) / 1024f, 4);
        }
    }
}