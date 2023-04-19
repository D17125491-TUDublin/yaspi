using Microsoft.Extensions.Configuration;
using yaspi.common;

namespace yaspi.integration.facebook;

public class TestFacebookApiService : IFacebookApiService
{
    public int ConnectorId { get { return 2; } }
    private readonly IEventBus _eventBus;
    private readonly IConfiguration _configuration;

    public TestFacebookApiService(IEventBus eventBus, IConfiguration configuration)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }

    public string GetAuthRequestUrl()
    {
        throw new NotImplementedException();
    }

    public KeyValuePair<string, string>[] GetConnectionData(string token)
    {
        throw new NotImplementedException();
    }

    public void SendMessage(YaspiConnection connection, YaspiMessage message)
    {
        UserMessage um = new UserMessage();
        um.Body = message.Text;
        um.SourceName = "Facebook - Test";
        um.Subject = "Test Facebook Message";
        um.UserName = connection.Username;
        SendUserMessageEvent messageEvent = new SendUserMessageEvent(connection.YaspiConnectionId, um);
        _eventBus.Publish(messageEvent);
        YaspiMessageSentSuccessEvent messageSentEvent = new YaspiMessageSentSuccessEvent(message.YaspiMessageId, "location");
        _eventBus.Publish(messageSentEvent);
    }
}