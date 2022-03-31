using System;
using FakeDataGenerator.Enums;
using FakeDataGenerator.Models.General;

namespace FakeDataGenerator.Processes
{
    public class DataProcessFactory
    {
        public IDataProcess GetProcess(ArgumentOptions argumentEntity)
        {
            if (argumentEntity.EntityNameForMapping == ClassNameForMapping.Instant)
                return new GenerateInstantEntity();

            if (argumentEntity.EntityNameForMapping == ClassNameForMapping.TableUser)
                return new GenerateTableUserEntity();
            
            throw new Exception();
        }
    }
}