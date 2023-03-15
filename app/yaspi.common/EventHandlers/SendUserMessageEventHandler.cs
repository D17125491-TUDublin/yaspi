using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class SendUserMessageEventHandler : IEventHandler<SendUserMessageEvent> 
{
        private readonly IConfiguration _configuration;
        private readonly IEventBus _eventBus;

        public SendUserMessageEventHandler(IConfiguration configuration, IEventBus eventBus)
        {
            _eventBus = eventBus;
            _configuration = configuration;
        }

        public void Handle(SendUserMessageEvent @event)
        {
            var msg = @event.Message;
            using (var sqlConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                sqlConnection.Open();
                var command = new SqlCommand("INSERT INTO UserMessage ([UserName],[Body],[SourceName],Subject) VALUES (@UserName,@Body,@SourceName,@Subject)", sqlConnection);
                command.CommandType = System.Data.CommandType.Text;
                command.Parameters.AddWithValue("@UserName", msg.UserName);
                command.Parameters.AddWithValue("@Body", msg.Body);
                command.Parameters.AddWithValue("@SourceName", msg.SourceName);
                command.Parameters.AddWithValue("@Subject", msg.Subject);
                command.ExecuteNonQuery();
            }
        }
}