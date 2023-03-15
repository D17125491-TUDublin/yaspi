using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class YaspiConnectionDataUpdateEventHandler : IEventHandler<YaspiConnectionDataUpdateEvent>
{

    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public YaspiConnectionDataUpdateEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public void Handle(YaspiConnectionDataUpdateEvent e)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var transaction = sqlConnection.BeginTransaction();
            var command = new SqlCommand(
                @"UPDATE YaspiConnection SET 
                    Modified = CURRENT_TIMESTAMP 
                WHERE YaspiConnectionId = @YaspiConnectionId", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectionId", e.YaspiConnectionId);
            command.Transaction = transaction;
            command.ExecuteNonQuery();
            foreach(var data in e.YaspiConnectionData)
            {
                command = new SqlCommand(
                    @"UPDATE YaspiConnectionData SET 
                        Value = @Value
                    WHERE 
                        [Key] = @Key
                    AND [YaspiConnectionId] = @YaspiConnectionId", sqlConnection);
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.AddWithValue("@YaspiConnectionId", e.YaspiConnectionId);
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