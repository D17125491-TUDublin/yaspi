using Microsoft.Extensions.Configuration;

namespace yaspi.common;

public class PostToAllActiveConnectionsEventHandler : IEventHandler<PostToAllActiveConnectionsEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly string _connectionString;

    public PostToAllActiveConnectionsEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _eventBus = eventBus;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public void Handle(PostToAllActiveConnectionsEvent _event)
    {
        ListYaspiConnectionsQuery query = new ListYaspiConnectionsQuery(_event.Username, _connectionString);
        IEnumerable<YaspiConnection> c = query.Execute();
        foreach (var item in c)
        {
            if (item.IsActive)
            {
                SendYaspiMessageEvent @event = new SendYaspiMessageEvent(
                    new YaspiMessage()
                    {
                        YaspiConnectionId = item.YaspiConnectionId,
                        Text = _event.Message
                    });
                _eventBus.Publish(@event);
            }
        }
    }
}