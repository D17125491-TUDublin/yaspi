using Microsoft.Data.SqlClient;

namespace yaspi.common.Queries;

public class GetYaspiMessagesByConnectorIdQuery : IQuery<IEnumerable<YaspiMessage>>
{
    public int _yaspiConnectorId { get; set; }
    private readonly string _connectionString;
    private readonly bool _isSent;
    private readonly bool _hasError;

    public GetYaspiMessagesByConnectorIdQuery(int yaspiConnectorId, bool isSent, bool hasError, string connectionString)
    {
        _yaspiConnectorId = yaspiConnectorId;
        _connectionString = connectionString;
        _isSent = isSent;
        _hasError = hasError;
    }

    public IEnumerable<YaspiMessage> Execute()
    {
        var yaspiMessages = new List<YaspiMessage>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = new SqlCommand(@"SELECT m.*,c.YaspiConnectorId,c.YaspiConnectionName FROM YaspiMessage m
                                            left join YaspiConnection c on c.YaspiConnectionId = m.YaspiConnectionId
                                            WHERE YaspiConnectorId = @YaspiConnectorId AND IsSent = @IsSent AND m.HasError = @HasError", connection);
            command.Parameters.AddWithValue("@YaspiConnectorId", _yaspiConnectorId);
            command.Parameters.AddWithValue("@IsSent", _isSent);
            command.Parameters.AddWithValue("@HasError", _hasError);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var yaspiMessage = new YaspiMessage
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
                yaspiMessages.Add(yaspiMessage);
            }
        }
        return yaspiMessages;
    }
}