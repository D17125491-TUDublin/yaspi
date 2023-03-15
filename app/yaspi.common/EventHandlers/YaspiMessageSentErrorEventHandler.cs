using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

namespace yaspi.common;

public class YaspiMessageSentErrorEventHandler : IEventHandler<YaspiMessageSentErrorEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly string _connectionString;
    public YaspiMessageSentErrorEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }
    
    public void Handle(YaspiMessageSentErrorEvent e)
    {
        using (var sqlConnection = new SqlConnection(_connectionString))
        {
            sqlConnection.Open();
            var command = new SqlCommand("UPDATE YaspiMessage SET IsSent = @IsSent, ErrorMessage = @ErrorMessage, HasError = @HasError, Modified = CURRENT_TIMESTAMP WHERE YaspiMessageId = @YaspiMessageId", sqlConnection);
            command.Parameters.AddWithValue("@YaspiMessageId", e.MessageId);
            command.Parameters.AddWithValue("@ErrorMessage", e.ErrorMessage);
            command.Parameters.AddWithValue("@HasError", true);
            command.Parameters.AddWithValue("@IsSent", false);
            command.ExecuteNonQuery();
        }

        var  yaspiMessage = new GetYaspiMessageByIdQuery(e.MessageId, _connectionString).Execute();
        var yaspiConnection = new GetYaspiConnectionByIdQuery(yaspiMessage.YaspiConnectionId, _connectionString).Execute();

        UserMessage userMessage = new UserMessage();
        userMessage.Body = e.ErrorMessage;
        userMessage.SourceName = yaspiConnection.YaspiConnectionName;
        userMessage.Subject = "Your message was not sent.";
        userMessage.UserName = yaspiConnection.Username;
        userMessage.TypeId = UserMessage.TYPE_ERROR;
        SendUserMessageEvent sendUserMessageEvent = new SendUserMessageEvent(e.MessageId, userMessage);
        _eventBus.Publish(sendUserMessageEvent);
    }
}
