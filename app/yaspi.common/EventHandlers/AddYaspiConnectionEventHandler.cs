namespace yaspi.common;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

public class AddYaspiConnectionEventHandler : IEventHandler<AddYaspiConnectionEvent>
{

    private IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public AddYaspiConnectionEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _eventBus = eventBus;
    }

    public void Handle(AddYaspiConnectionEvent @event)
    {
        var connection = @event.Connection;
        string correlationId = Guid.NewGuid().ToString();
        if (string.IsNullOrWhiteSpace(connection.YaspiConnectionName)) connection.YaspiConnectionName = Guid.NewGuid().ToString();

        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            SqlTransaction transaction = sqlConnection.BeginTransaction();
            var command = new SqlCommand("INSERT INTO YaspiConnection (YaspiConnectionName, YaspiConnectorId, UserName) VALUES (@YaspiConnectionName, @YaspiConnectorId, @UserName); SELECT SCOPE_IDENTITY()", sqlConnection);
            command.Transaction = transaction;
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectionName", connection.YaspiConnectionName);
            command.Parameters.AddWithValue("@YaspiConnectorId", connection.YaspiConnectorId);
            command.Parameters.AddWithValue("@UserName", connection.Username);
            int scope_id = Convert.ToInt32(command.ExecuteScalar());
            if (connection.ConnectionData != null)
                foreach (var item in connection.ConnectionData)
                {
                    command = new SqlCommand("INSERT INTO YaspiConnectionData ([YaspiConnectionId], [Key], [Value]) VALUES (@YaspiConnectionId, @Key, @Value)", sqlConnection);
                    command.Transaction = transaction;
                    command.CommandType = System.Data.CommandType.Text;
                    command.Parameters.AddWithValue("@YaspiConnectionId", scope_id);
                    command.Parameters.AddWithValue("@Key", item.Key);
                    command.Parameters.AddWithValue("@Value", item.Value);
                    command.ExecuteNonQuery();
                }
            transaction.Commit();
            sqlConnection.Close();
        }
    }
}