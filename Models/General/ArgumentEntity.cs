using System;
using System.Linq;
using FakeDataGenerator.Enums;

namespace FakeDataGenerator.Models.General
{
    public class ArgumentEntity
    {
        public ArgumentEntity(string[] args)
        {
            ArgsPreprocessing(args);
        }


        public int AmountOfGeneratedData { get; set; } = 100;
        public FileExtensions SaveAsExtension { get; set; } = FileExtensions.JsonExtension;
        public ClassNameForMapping EntityNameForMapping { get; set; } = ClassNameForMapping.Instant;
        
        private void ArgsPreprocessing(string[] args)
        {
            if (args.Length <= 0) return;
            
            if (args[0].All(char.IsDigit)) AmountOfGeneratedData = int.Parse(args[0]);

            if (args[1].Length > 5)
            {
                if (args[1].Contains("instant", StringComparison.OrdinalIgnoreCase)) EntityNameForMapping = ClassNameForMapping.Instant;
                if (args[1].Contains("tableuser", StringComparison.OrdinalIgnoreCase)) EntityNameForMapping = ClassNameForMapping.TableUser;
            }
            
            if (args[2].Length > 0)
            {
                if (args[2].Contains("csv", StringComparison.OrdinalIgnoreCase)) SaveAsExtension = FileExtensions.CsvExtension;
                if (args[2].Contains("json", StringComparison.OrdinalIgnoreCase)) SaveAsExtension = FileExtensions.JsonExtension;
            }
        }
    }
}