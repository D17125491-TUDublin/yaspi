using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class YaspiMessageSentSuccessEventHandler : IEventHandler<YaspiMessageSentSuccessEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    public YaspiMessageSentSuccessEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }
    
    public void Handle(YaspiMessageSentSuccessEvent e)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"))) {
            sqlConnection.Open();
            var command = new SqlCommand("UPDATE YaspiMessage SET IsSent = @IsSent,Location=@Location, Modified = CURRENT_TIMESTAMP WHERE YaspiMessageId = @YaspiMessageId", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiMessageId", e.MessageId);
            command.Parameters.AddWithValue("@Location", e.Location);
            command.Parameters.AddWithValue("@IsSent", true);
            command.ExecuteNonQuery();
        }
    } 
}
