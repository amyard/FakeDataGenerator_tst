using System.Collections.Generic;
using CsvHelper.Configuration;
using FakeDataGenerator.Models.Instant;

namespace FakeDataGenerator.Models.CsvMapping
{
    public class InstantClassMap : ClassMap<InstantPaycardInfo>
    {
        public InstantClassMap()
        {
            Map(i => i.EnternalLocationId).Name("EnternalLocationId");
            Map(i => i.EmployeeInfo.EmployeeId);
            Map(i => i.EmployeeInfo.FirstName);
            Map(i => i.EmployeeInfo.LastName);
            Map(i => i.EmployeeInfo.BirthDate);
            Map(i => i.EmployeeInfo.BlockInstantPay);
            Map(i => i.EmployeeInfo.Terminated);
            Map(i => i.EmployeeInfo.PaidOnInstant);
            Map(i => i.JobInfo).TypeConverter<JsonConverter<List<JobInfo>>>();
        }
    }
}