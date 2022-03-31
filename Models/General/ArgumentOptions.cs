using CommandLine;
using FakeDataGenerator.Enums;

namespace FakeDataGenerator.Models.General
{
    public class ArgumentOptions
    {
        
        [Option('a', "amountData", Required = false, HelpText = "Set the amount of generated data.")]
        public int AmountOfGeneratedData { get; set; } = 100;
        
        [Option('e', "extension", Required = false, HelpText = "Set file extension for saving (case-sensitive parameter).")]
        public FileExtensions SaveAsExtension { get; set; } = FileExtensions.Json;
        
        [Option('m', "model", Required = false, HelpText = "Set model name as enum (case-sensitive parameter).")]
        public ClassNameForMapping EntityNameForMapping { get; set; } = ClassNameForMapping.Instant;
    }
}