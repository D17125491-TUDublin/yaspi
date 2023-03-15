using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetUserMessagesQuery :IQuery<List<UserMessage>>{
    private readonly string _userName;
    private readonly string _connectionString;
    public GetUserMessagesQuery(string userName, string connectionString) {
        _userName = userName;
        _connectionString = connectionString;
    }
    public List<UserMessage> Execute() {
        var messages = new List<UserMessage>();
        using (var connection = new SqlConnection(_connectionString)) {
            connection.Open();
            var command = new SqlCommand("SELECT * FROM UserMessage WHERE UserName = @UserName", connection);
            command.Parameters.AddWithValue("@UserName", _userName);
            var reader = command.ExecuteReader();
            while (reader.Read()) {
                var message = new UserMessage();
                message.UserMessageId = (int)reader["UserMessageId"];
                message.Subject = (string)reader["Subject"];
                message.Body = (string)reader["Body"];
                message.UserName = (string)reader["UserName"];
                message.SourceName = (string)reader["SourceName"];
                message.Created = (DateTime)reader["Created"];
                message.IsRead = (bool)reader["IsRead"];
                messages.Add(message);
            }
        }
        return messages;
    }
}