using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
namespace yaspi.common;

public class SetYaspiConnectionActiveEventHandler : IEventHandler<SetYaspiConnectionActiveEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public SetYaspiConnectionActiveEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public void Handle(SetYaspiConnectionActiveEvent @event)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var command = new SqlCommand("UPDATE YaspiConnection SET IsActive = @IsActive, Modified=CURRENT_TIMESTAMP WHERE YaspiConnectionId = @YaspiConnectionId", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectionId", @event.YaspiConnectionId);
            command.Parameters.AddWithValue("@IsActive", @event.IsActive);
            command.ExecuteNonQuery();
        }
    }
}