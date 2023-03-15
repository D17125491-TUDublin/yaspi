using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class AddYaspiConnectorEventHadler : IEventHandler<AddYaspiConnectorEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;

    public AddYaspiConnectorEventHadler(IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _eventBus = eventBus;
    }

    public void Handle(AddYaspiConnectorEvent _event)
    {
        var connector = _event.Connector;
        using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        {
            sqlConnection.Open();
            var command = new SqlCommand("INSERT INTO YaspiConnector ([YaspiConnectorName],Base64SmallIcon,Base64LargeIcon,RedirectUrl,Description) VALUES (@YaspiConnectorName,@Base64SmallIcon,@Base64LargeIcon,@RedirectUrl,@Description)", sqlConnection);
            command.CommandType = System.Data.CommandType.Text;
            command.Parameters.AddWithValue("@YaspiConnectorName", connector.YaspiConnectorName);
            command.Parameters.AddWithValue("@Description", connector.Description);
            command.Parameters.AddWithValue("@RedirectUrl", connector.RedirectUrl);
            command.Parameters.AddWithValue("@Base64LargeIcon", connector.Base64LargeIcon);
            command.Parameters.AddWithValue("@Base64SmallIcon", connector.Base64SmallIcon);
            command.ExecuteNonQuery();
        }
    }
}