using System;

namespace FakeDataGenerator.Models.Person
{
    public class TableUser
    {
        public TableUser(string customerID)
        {
            this.CustomerID = customerID;
        }
        public string CustomerID { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public bool NameStyle { get; set; }
        public string Suffix { get; set; }
        public string CompanyName { get; set; }
        public string SalesPerson { get; set; }
        public string EmailAddress { get; set; }
        public string Phone { get; set; }
    }
}