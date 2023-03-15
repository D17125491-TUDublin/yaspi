using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetUnreadMessagesCountQuery : IQuery<int>
{
    private string _userName { get; set; }
    private string _connectionString { get; set; }
    public GetUnreadMessagesCountQuery(string userName, string connectionString)
    {
        _userName = userName;
        _connectionString = connectionString;
    }
    public int Execute()
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand("SELECT COUNT(1) FROM UserMessage WHERE UserName = @UserName AND IsRead = 0", connection);
            command.Parameters.AddWithValue("@UserName", _userName);
            var count = (int)command.ExecuteScalar();
            return count;
        }
    }
}