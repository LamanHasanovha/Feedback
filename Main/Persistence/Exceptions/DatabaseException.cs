namespace Main.Persistence.Exceptions;

public class DatabaseException : Exception
{
    public Exception OccuredException { get; set; }
    public DatabaseException(Exception exception, string message = "An error occured while executing SQL command") : base(message, exception)
    {
    }
}
