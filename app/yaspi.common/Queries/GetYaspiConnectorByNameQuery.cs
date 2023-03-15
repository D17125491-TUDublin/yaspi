namespace yaspi.common;
using Microsoft.Data.SqlClient;

public class GetYaspiConnectorByNameQuery : IQuery<YaspiConnector> {
    private readonly string _connectionString;
    private readonly string _name;
    public GetYaspiConnectorByNameQuery(string name, string connectionString)
    {
        _connectionString = connectionString;
        _name = name;
    }
    public YaspiConnector Execute()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM YaspiConnector WHERE YaspiConnectorName = @YaspiConnectorName", connection);
            command.Parameters.AddWithValue("@YaspiConnectorName", _name);
            command.CommandType = System.Data.CommandType.Text;
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new YaspiConnector
                {
                    YaspiConnectorId = (int)reader["YaspiConnectorId"],
                    YaspiConnectorName = (string)reader["YaspiConnectorName"],
                    Description = (string)reader["Description"],
                    RedirectUrl = (string)reader["RedirectUrl"],
                    Base64LargeIcon = (string)reader["Base64LargeIcon"],
                    Base64SmallIcon = (string)reader["Base64SmallIcon"],
                    IsActive = (bool)reader["IsActive"]
                };
            }
        }
        return null;
    }
}