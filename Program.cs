using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using FakeDataGenerator.Models.General;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace FakeDataGenerator
{
    class Program
    {
        private static IFakeDataGenerator _fakeDataGenerator;
        private static ILogger _logger;
        private static ArgumentOptions _argumentOptions = new ArgumentOptions();
        private static readonly StringBuilder _errorStringBuilder = new StringBuilder();

        static async Task Main(string[] args)
        {
            var host = Startup.AppStartup(args);
            _fakeDataGenerator = host.Services.GetService<IFakeDataGenerator>();
            _logger = host.Services.GetService<ILogger>();
            
            int exitCode = Parser.Default.ParseArguments<ArgumentOptions>(args)
                .MapResult((ArgumentOptions opts) => RunOptionsSuccess(opts),
                            errs => HandleParseError(errs));

            if (_errorStringBuilder.Length > 0)
            {
                _logger.Error(_errorStringBuilder.ToString());
                Environment.Exit(exitCode);
            }

            await StartAsync(_argumentOptions);
        }

        private static async Task StartAsync(ArgumentOptions argumentEntity)
        {
            try
            {
                await _fakeDataGenerator.GeneratedDataAsync(argumentEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Issue occur during processing. {ex.Message}");
            }
        }
        
        static int RunOptionsSuccess(ArgumentOptions options)
        {
            var exitCode = 0;
            _argumentOptions = options;
            
            return exitCode;
        }

        /// <summary>
        /// Error handler process. in case of errors or --help or --version
        /// </summary>
        /// <param name="errs">List of errors</param>
        /// <returns>Error code</returns>
        static int HandleParseError(IEnumerable<Error> errs)
        {
            var result = -2;
            
            if (errs.Any(x => x is HelpRequestedError or VersionRequestedError)) result = -1;

            foreach (var errorItem in errs)
            {
                switch (errorItem)
                {
                    case BadFormatConversionError:
                        _errorStringBuilder.Append($"Parser Error - {errorItem.Tag}. \n");
                        break;
                    case UnknownOptionError error:
                        _errorStringBuilder.Append($"Option \'{error?.Token}\' is unknown. \n");
                        break;
                    default:
                        _errorStringBuilder.Append($"Parser Error - {errorItem.Tag}. Unknown error. \n");
                        break;
                }
            }
            
            return result;
        }
    }
}