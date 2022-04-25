using CsvHelper.Configuration;
using FakeDataGenerator.Models.Person;

namespace FakeDataGenerator.Models.CsvMapping
{
    public class TableUserClassMap : ClassMap<TableUser>
    {
        public TableUserClassMap()
        {
            Map(u => u.CustomerID);
            Map(u => u.ModifiedDate);
            Map(u => u.Title);
            Map(u => u.FirstName);
            Map(u => u.LastName);
            Map(u => u.MiddleName);
            Map(u => u.NameStyle);
            Map(u => u.Suffix);
            Map(u => u.CompanyName);
            Map(u => u.SalesPerson);
            Map(u => u.EmailAddress);
            Map(u => u.Phone);
        }
    }
}