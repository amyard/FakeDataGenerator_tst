using System.Collections.Generic;
using System.Linq;
using Bogus;
using FakeDataGenerator.Models.Person;

namespace FakeDataGenerator.Processes
{
    public class GenerateTableUserEntity: IDataProcess
    {
        public List<object> GenerateFakeDataEntities(int amountOfGeneratedData)
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
            
            var personalData = userFaker.Generate(amountOfGeneratedData);

            return personalData.Cast<object>().ToList();
        }
    }
}