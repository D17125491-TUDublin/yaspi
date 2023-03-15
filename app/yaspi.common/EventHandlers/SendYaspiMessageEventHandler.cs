using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;
public class SendYaspiMessageEventHandler : IEventHandler<SendYaspiMessageEvent>
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IEventBus _eventBus;

    public SendYaspiMessageEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public void Handle(SendYaspiMessageEvent @event)
    {
        var message = @event.Message;
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var command = new SqlCommand("INSERT INTO YaspiMessage ([YaspiConnectionId],[Text]) VALUES (@YaspiConnectionId,@Text)", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectionId", message.YaspiConnectionId);
            command.Parameters.AddWithValue("@Text", message.Text);
            command.ExecuteNonQuery();
        }
    }
}
