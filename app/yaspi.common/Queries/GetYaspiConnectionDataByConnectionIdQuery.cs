using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetYaspiConnectionDataByConnectionQueryId {
    private readonly int _id;
    private readonly string _connectionString;
    public GetYaspiConnectionDataByConnectionQueryId(int id, string connectionString) {
        _id = id;
        _connectionString = connectionString;
    }
    public KeyValuePair<string, string>[] Execute() {
        using (var connection = new SqlConnection(_connectionString)) {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM [dbo].[YaspiConnectionData] WHERE [YaspiConnectionId] = @id", connection);
            command.Parameters.AddWithValue("@id", _id);
            var reader = command.ExecuteReader();
            var result = new List<KeyValuePair<string, string>>();
            while (reader.Read()) {
                result.Add(new KeyValuePair<string, string>((string)reader["Key"], (string)reader["Value"]));
            }
            return result.ToArray();
        }
    }
}