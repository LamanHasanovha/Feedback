namespace Main.Persistence.PersistenceBase;

public interface IConnectionStringProvider
{
    string GetMainConnectionString();

    string GetConnectionString(string name);
}
