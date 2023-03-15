using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class SetUserMessageReadEventHandler : IEventHandler<SetUserMessageReadEvent> 
{
     private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public SetUserMessageReadEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public void Handle(SetUserMessageReadEvent @event)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var command = new SqlCommand("UPDATE UserMessage SET IsRead = @IsRead, Modified=CURRENT_TIMESTAMP WHERE UserMessageId = @UserMessageId", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@UserMessageId", @event.UserMessageId);
            command.Parameters.AddWithValue("@IsRead", true);
            command.ExecuteNonQuery();
        }
    }
}