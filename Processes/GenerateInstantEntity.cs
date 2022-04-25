using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using FakeDataGenerator.Models.Instant;

namespace FakeDataGenerator.Processes
{
    public class GenerateInstantEntity: IDataProcess
    {
        public List<object> GenerateFakeDataEntities(int amountOfGeneratedData)
        {
            Random random = new Random();
            string[] _externalLocationIds = new string[] {"123", "321"};
            string externalLocId = _externalLocationIds[random.Next(_externalLocationIds.Length)];
            
            var jobInfos = new Faker<JobInfo>()
                .RuleFor(o => o.HourlyRate, f => f.Finance.Amount(0, 20).OrNull(f, .5f))
                .RuleFor(o => o.AnnualSalary, (f, o) => o.HourlyRate == null ? o.AnnualSalary = f.Finance.Amount(0, 10000) : null)
                .RuleFor(o => o.Code, f => f.Random.AlphaNumeric(random.Next(10, 20)).OrNull(f, .2f))
                .RuleFor(o => o.IsHourly, f => f.Random.Bool())
                .RuleFor(o => o.IsDefault, f => false);

            var terminated = new Faker<Terminated>()
                .RuleFor(t => t.IsTerminated, f => f.Random.Bool())
                .Rules((f, t) => t.TerminatedData = !t.IsTerminated ? f.Date.Recent(30).ToString("yyyyy-MM-dd") : null);

            var employeeInfo = new Faker<EmployeeInfo>()
                .RuleFor(o => o.EmployeeId, f => f.Random.AlphaNumeric(random.Next(18, 99)))
                .RuleFor(o => o.FirstName, f => f.Name.FirstName())
                .RuleFor(o => o.LastName, f => f.Name.LastName())
                .RuleFor(o => o.BirthDate, f => f.Date.Past(70, DateTime.Now.AddYears(-20)).ToString("yyyy-MM-dd"))
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
                .RuleFor(o => o.EmployeeInfo, f => employeeInfo.Generate())
                .RuleFor(o => o.JobInfo, (f, employee) =>
                {
                    var jobinfos = jobInfos.Generate(random.Next(1, 5)).ToArray();
                    // set one value in array to be true
                    f.Random.ArrayElement(jobinfos).IsDefault = true;
                    jobinfos.ToList().ForEach(x => x.Name = $"{employee.EmployeeInfo.LastName} {employee.EmployeeInfo.FirstName}");

                    return jobinfos;
                });

            List<InstantPaycardInfo> instantData = instantEntity.Generate(amountOfGeneratedData);
            

            return instantData.Cast<object>().ToList();
        }
    }
}