using System.Data;
using MySqlConnector;

public class MySqlFactory : ConnectionFactory
{
    public MySqlFactory(IConfiguration _configuration) : base(_configuration)
    {
    }

    public override IDbConnection Connect()
    {
        string connectionString = configuration.GetConnectionString("MySql"); //nome informado em appsetings na ConnectionString
        connection = new MySqlConnection(connectionString);
        connection.Open();
        return connection;
    }
}