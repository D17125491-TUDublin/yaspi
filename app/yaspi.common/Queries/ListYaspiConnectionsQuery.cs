namespace yaspi.common;
using Microsoft.Data.SqlClient;
public class ListYaspiConnectionsQuery : IQuery<IEnumerable<YaspiConnection>>
{
    private string _connectionString;
    private string _userName; 
    public ListYaspiConnectionsQuery(string userName, string connectionString)
    {
        _connectionString = connectionString;
        _userName = userName;
    }

    public IEnumerable<YaspiConnection> Execute()
    {
        List<YaspiConnection> connections = new List<YaspiConnection>();
        using(var connection = new SqlConnection(_connectionString)){
            connection.Open();
            var command = new SqlCommand(@"SELECT * FROM YaspiConnection co
                                        left join YaspiConnector cr on cr.YaspiConnectorId = co.YaspiConnectorId
                                        WHERE UserName = @UserName", connection);
            command.Parameters.AddWithValue("@UserName", _userName);
            command.CommandType = System.Data.CommandType.Text;
            var reader = command.ExecuteReader();
            while(reader.Read()){
                connections.Add(new YaspiConnection{
                    YaspiConnectionId = (int)reader["YaspiConnectionId"],
                    YaspiConnectionName = (string)reader["YaspiConnectionName"],
                    YaspiConnectorName = (string)reader["YaspiConnectorName"],
                    YaspiConnectorId = (int)reader["YaspiConnectorId"],
                    Username = (string)reader["UserName"],
                    IsActive = (bool)reader["IsActive"]
                });
            }
        }
        return connections;
    }
}