using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetYaspiApiKeyQuery : IQuery<YaspiApiKey>
{

    private readonly string _username;
    private readonly string _connectionString;
    public GetYaspiApiKeyQuery(string username, string connectionString)
    {
        _username = username;
        _connectionString = connectionString;
    }

    public YaspiApiKey Execute()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM [dbo].[YaspiApiKey] WHERE [Username] = @username", connection);
            command.Parameters.AddWithValue("@username", _username);
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var token = new YaspiApiKey();
                token.YaspiApiKeyId = (int)reader["YaspiApiKeyId"];
                token.Username = (string)reader["Username"];
                token.Key = (string)reader["Key"];
                token.IsActive = (bool)reader["IsActive"];
                return token;
            }
            return null;
        }
    }
}