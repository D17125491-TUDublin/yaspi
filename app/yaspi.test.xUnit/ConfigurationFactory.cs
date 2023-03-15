using Microsoft.Extensions.Configuration;

public class ConfigurationFactory
{
    public static IConfiguration GetConfiguration()
    {
         IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();
        return configuration;        
    }
}