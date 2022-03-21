namespace FakeDataGenerator.Models.Instant
{
    public class JobInfo
    {
        public string Name { get; set; }
        public decimal? HourlyRate { get; set; }
        public decimal? AnnualSalary { get; set; }
        public string Code { get; set; }
        public bool IsHourly { get; set; }
        public bool IsDefault { get; set; }
    }
}