using System.Diagnostics;
using System.Reflection;

namespace FakeDataGenerator
{
    internal static class ApplicationInfo
    {
        static ApplicationInfo()
        {
            var assembly = Assembly.GetExecutingAssembly();
            ApplicationName = assembly.GetName().Name;
            ApplicationVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;

        }

        public static string ApplicationName { get; }
        public static string ApplicationVersion { get; }
    }
}