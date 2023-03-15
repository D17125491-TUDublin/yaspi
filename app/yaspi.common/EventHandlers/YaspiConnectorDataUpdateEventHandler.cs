using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class YaspiConnectorDataUpdateEventHandler : IEventHandler<YaspiConnectorDataUpdateEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public YaspiConnectorDataUpdateEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public void Handle(YaspiConnectorDataUpdateEvent e)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var transaction = sqlConnection.BeginTransaction();
            var command = new SqlCommand(
                @"UPDATE YaspiConnector SET 
                    Modified = CURRENT_TIMESTAMP 
                WHERE YaspiConnectorId = @YaspiConnectorId", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectorId", e.YaspiConnectorId);
            command.Transaction = transaction;
            command.ExecuteNonQuery();
            foreach(var data in e.YaspiConnectorData)
            {
                command = new SqlCommand(
                    @"UPDATE YaspiConnectorData SET 
                        Value = @Value
                    WHERE 
                        [Key] = @Key
                    AND [YaspiConnectorId] = @YaspiConnectorId", sqlConnection);
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.AddWithValue("@YaspiConnectorId", e.YaspiConnectorId);
                command.Parameters.AddWithValue("@Value", data.Value);
                command.Parameters.AddWithValue("@Key", data.Key);
                command.Transaction = transaction;
                command.ExecuteNonQuery();
            }
            transaction.Commit();
            sqlConnection.Close();
        }
    }
}