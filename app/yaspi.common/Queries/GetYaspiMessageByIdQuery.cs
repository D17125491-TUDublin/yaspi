using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class GetYaspiMessageByIdQuery : IQuery<YaspiMessage> {
    public int YaspiMessageId { get; set; }
    private readonly string _connectionString;
    public GetYaspiMessageByIdQuery(int messageId,string connectionString) {
        YaspiMessageId = messageId;
        _connectionString = connectionString;
    }

    public YaspiMessage Execute() {
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(@"SELECT m.*,c.YaspiConnectorId,c.YaspiConnectionName FROM YaspiMessage m
                                            left join YaspiConnection c on c.YaspiConnectionId = m.YaspiConnectionId
                                            WHERE m.YaspiMessageId = @YaspiMessageId", connection);
            command.Parameters.AddWithValue("@YaspiMessageId", YaspiMessageId);

            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new YaspiMessage
                {
                    YaspiMessageId = (int)reader["YaspiMessageId"],
                    YaspiConnectorId = (int)reader["YaspiConnectorId"],
                    YaspiConnectionId = (int)reader["YaspiConnectionId"],
                    YaspiConnectionName = (string)reader["YaspiConnectionName"],
                    Text = (string)reader["Text"],
                    IsSent = (bool)reader["IsSent"],
                    Created = (DateTime)reader["Created"],
                    Modified = (DateTime)reader["Modified"],
                    Location = reader["Location"] == DBNull.Value ? null : (string)reader["Location"],
                    HasError = (bool)reader["HasError"],
                    ErrorMessage = reader["ErrorMessage"] == DBNull.Value ? null : (string)reader["ErrorMessage"]
                };
            }
            return null;
        }

    }
}