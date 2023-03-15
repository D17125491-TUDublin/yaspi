using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class GenerateYaspiApiKeyEventHandler : IEventHandler<GenerateYaspiApiKeyEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public GenerateYaspiApiKeyEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _eventBus = eventBus;
    }

    public void Handle(GenerateYaspiApiKeyEvent _event)
    {
        string key = generateKey();
        string username = _event.Username;
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var command = new SqlCommand(@"
                UPDATE YaspiApiKey set [Key] = @Key WHERE Username = @Username
                IF @@ROWCOUNT = 0
                    INSERT INTO YaspiApiKey (UserName, [Key]) VALUES (@UserName, @Key)
                ", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@Key", key);
            command.Parameters.AddWithValue("@UserName", username);
            command.ExecuteNonQuery();
        }
    }

    private string generateKey(){
        string newKey = Guid.NewGuid().ToString();
        return newKey;
    }
}