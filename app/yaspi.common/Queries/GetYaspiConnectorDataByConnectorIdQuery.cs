using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetYaspiConnectorDataByConnectorIdQuery : IQuery<KeyValuePair<string,string>[]> {
    private int _yaspiConnectorId { get; set; }
    private string _connectionString { get; set; }
    

    public GetYaspiConnectorDataByConnectorIdQuery(int yaspiConnectorId, string connectionString) {
        _yaspiConnectorId = yaspiConnectorId;
        _connectionString = connectionString;
    }
    public KeyValuePair<string,string>[] Execute() {
        using (var connection = new SqlConnection(_connectionString)) {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM [dbo].[YaspiConnectorData] WHERE [YaspiConnectorId] = @id", connection);
            command.Parameters.AddWithValue("@id", _yaspiConnectorId);
            var reader = command.ExecuteReader();
            var result = new List<KeyValuePair<string, string>>();
            while (reader.Read()) {
                result.Add(new KeyValuePair<string, string>((string)reader["Key"], (string)reader["Value"]));
            }
            return result.ToArray();
        }
    }
}