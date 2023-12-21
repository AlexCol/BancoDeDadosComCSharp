using System.Data;

public abstract class ConnectionFactory : IConnectionFactory
{
    protected readonly IConfiguration configuration;
    protected IDbConnection connection;

    public ConnectionFactory(IConfiguration _configuration)
    {
        configuration = _configuration;
    }

    public abstract IDbConnection Connect();

    public void Dispose()
    {
        connection.Close();
    }
}
