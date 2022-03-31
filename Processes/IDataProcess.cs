using System.Collections.Generic;

namespace FakeDataGenerator.Processes
{
    public interface IDataProcess
    {
        List<object> GenerateFakeDataEntities(int amountOfGeneratedData);
    }
}