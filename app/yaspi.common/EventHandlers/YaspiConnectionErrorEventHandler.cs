using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class YaspiConnectionErrorEventHandler : IEventHandler<YaspiConnectionErrorEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly string connectionString;
    public YaspiConnectionErrorEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    public void Handle(YaspiConnectionErrorEvent e)
    {
         using (var sqlConnection = new SqlConnection(connectionString))
        {
            sqlConnection.Open();
            var command = new SqlCommand(@"UPDATE YaspiConnection 
                                SET HasError = @HasError, 
                                ErrorMessage = @ErrorMessage, 
                                Modified = CURRENT_TIMESTAMP,
                                [IsActive] = @IsActive
                         WHERE YaspiConnectionId = @YaspiConnectionId", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectionId", e.YaspiConnectionId);
            command.Parameters.AddWithValue("@ErrorMessage", e.ErrorMessage);
            command.Parameters.AddWithValue("@HasError", true);
            command.Parameters.AddWithValue("@IsActive", false);
            command.ExecuteNonQuery();
        }
        GetYaspiConnectionByIdQuery getYaspiConnectionByIdQuery = new GetYaspiConnectionByIdQuery(e.YaspiConnectionId, connectionString);
        var yaspiConnection = getYaspiConnectionByIdQuery.Execute();
        var userMessage = new UserMessage
        {
            Subject = $"Connection to {e.YaspiConnectionId} has error: {e.ErrorMessage}",
            Body = $"Connection to {e.YaspiConnectionId} has error: {e.ErrorMessage}",
            UserName = yaspiConnection.Username,
            SourceName = yaspiConnection.YaspiConnectionName,
            TypeId = UserMessage.TYPE_ERROR
        };
        SendUserMessageEvent sendUserMessageEvent = new SendUserMessageEvent(e.YaspiConnectionId, userMessage);
        _eventBus.Publish(sendUserMessageEvent);
    }
}