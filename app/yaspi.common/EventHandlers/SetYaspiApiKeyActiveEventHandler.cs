using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class SetYaspiApiKeyActiveEventHandler : IEventHandler<SetYaspiApiKeyActiveEvent> 
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public SetYaspiApiKeyActiveEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public void Handle(SetYaspiApiKeyActiveEvent _event)
    {
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
               sqlConnection.Open();
               var command = new SqlCommand(@"
                            UPDATE YaspiApiKey SET 
                                    IsActive = @IsActive, 
                                    [Updated]=CURRENT_TIMESTAMP 
                            WHERE YaspiApiKeyId = @YaspiApiKeyId", sqlConnection);   
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.AddWithValue("@YaspiApiKeyId", _event.YaspiApiKeyId);
                command.Parameters.AddWithValue("@IsActive", _event.IsActive);
                command.ExecuteNonQuery();
        }
    }
}