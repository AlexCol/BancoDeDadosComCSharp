using System.Data;
public interface IConnectionFactory
{
    IDbConnection Connect();
    void Dispose();
}
