using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace Ird.XunitAndMoq.Testing.Configuration;

public static class ConfigurationFactory
{
    public static IConfiguration GetConfiguration(Dictionary<string, string> configValues)
        => new ConfigurationBuilder()
                    .AddInMemoryCollection(configValues)
                    .Build();

    public static IConfiguration GetConfiguration(Assembly testAssembly, string basePath, string settingsFileName)
        => new ConfigurationBuilder()
                    .SetBasePath(basePath)
                    .AddJsonFile(settingsFileName)
                    .AddUserSecrets(testAssembly)
                    .Build();
}
