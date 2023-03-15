using Microsoft.Extensions.Configuration;
    using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class ListYaspiConnectorsQuery : IQuery<List<YaspiConnector>>
{
    private readonly string _connectionString;
    public ListYaspiConnectorsQuery(string connectionString)
    {
        _connectionString = connectionString;
    }
    public List<YaspiConnector> Execute()
    {
        var connectors = new List<YaspiConnector>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM YaspiConnector", connection);
            command.CommandType = System.Data.CommandType.Text;
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                connectors.Add(new YaspiConnector
                {
                    YaspiConnectorId = (int)reader["YaspiConnectorId"],
                    YaspiConnectorName = (string)reader["YaspiConnectorName"],
                    Description = (string)reader["Description"],
                    RedirectUrl = (string)reader["RedirectUrl"],
                    Base64LargeIcon = (string)reader["Base64LargeIcon"],
                    Base64SmallIcon = (string)reader["Base64SmallIcon"],
                    IsActive = (bool)reader["IsActive"]
                });
            }
        }
        return connectors;
    }
}