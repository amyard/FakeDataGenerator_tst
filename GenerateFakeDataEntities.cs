using System;
using System.Collections.Generic;
using Bogus;
using FakeDataGenerator.Models.Instant;
using FakeDataGenerator.Models.Person;

namespace FakeDataGenerator
{
    public interface IGenerateFakeDataEntities
    {
        List<InstantPaycardInfo> GenerateInstantEntity(int amountOfGeneratedData);
        List<TableUser> GenerateTableUserEntity(int amountOfGeneratedData);
    }
    
    public class GenerateFakeDataEntities : IGenerateFakeDataEntities
    {
        public List<InstantPaycardInfo> GenerateInstantEntity(int amountOfGeneratedData)
        {
            Random random = new Random();
            string[] _externalLocationIds = new string[] {"123", "321"};
            string externalLocId = _externalLocationIds[random.Next(_externalLocationIds.Length)];
            
            var jobInfo = new Faker<JobInfo>()
                .RuleFor(o => o.HourlyRate, f => f.Finance.Amount(0, 20).OrNull(f, .5f))
                .RuleFor(o => o.AnnualSalary, (f, o) => o.HourlyRate == null ? o.AnnualSalary = f.Finance.Amount(0, 10000) : null)
                .RuleFor(o => o.Code, f => f.Random.AlphaNumeric(random.Next(10, 20)).OrNull(f, .2f))
                .RuleFor(o => o.IsHourly, f => f.Random.Bool())
                .RuleFor(o => o.IsDefault, f => f.Random.Bool());

            var terminated = new Faker<Terminated>()
                .RuleFor(t => t.IsTerminated, f => f.Random.Bool())
                .Rules((f, t) =>
                {
                    t.TerminatedData = !t.IsTerminated ? f.Date.Recent(30).ToString("yyyyy-MM-dd") : null;
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

            List<InstantPaycardInfo> instantData = instantEntity.Generate(amountOfGeneratedData);
            
            instantData.ForEach(entity =>
            {
                foreach (var job in entity.JobInfo)
                {
                    job.Name = $"{entity.EmployeeInfo.LastName} {entity.EmployeeInfo.FirstName}";
                }
            });

            return instantData;
        }

        public List<TableUser> GenerateTableUserEntity(int amountOfGeneratedData)
        {
            var customerId = 1;

            var userFaker = new Faker<TableUser>()
                .CustomInstantiator(f => new TableUser(customerId++.ToString()))
                .RuleFor(o => o.ModifiedDate, f => f.Date.Recent(100))
                .RuleFor(o => o.NameStyle, f => f.Random.Bool())
                .RuleFor(o => o.Phone, f => f.Person.Phone)
                .RuleFor(o => o.FirstName, f => f.Name.FirstName())
                .RuleFor(o => o.LastName, f => f.Name.LastName())
                .RuleFor(o => o.Title, f => f.Name.Prefix(f.Person.Gender))
                .RuleFor(o => o.Suffix, f => f.Name.Suffix())
                .RuleFor(o => o.MiddleName, f => f.Name.FirstName())
                .RuleFor(o => o.EmailAddress, (f,u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(o => o.SalesPerson, f => f.Name.FullName())
                .RuleFor(o => o.CompanyName, f => f.Company.CompanyName());
            
            List<TableUser> personalData = userFaker.Generate(amountOfGeneratedData);

            return personalData;
        }
    }
}