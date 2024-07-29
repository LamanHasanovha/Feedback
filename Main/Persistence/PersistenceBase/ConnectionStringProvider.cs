using Microsoft.Extensions.Configuration;

namespace Main.Persistence.PersistenceBase;

public static class ConnectionStringProvider
{
    public static string GetMainConnectionString(IConfiguration configuration)
    {
        var value = configuration.GetConnectionString("Main");

        if (value != null)
            return value;

        throw new NullReferenceException("Specified connection string did not find");
    }

    public static string GetConnectionString(IConfiguration configuration, string name) 
    {
        var value = configuration.GetConnectionString(name);

        if (value != null) 
            return value;

        throw new NullReferenceException("Specified connection string did not find");
    }
}
