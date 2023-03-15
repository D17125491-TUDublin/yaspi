namespace yaspi.twitter;

using Microsoft.Extensions.Configuration;
using yaspi.common;

public class UserConnectedToTwitterEventHandler : IEventHandler<UserConnectedToTwitterEvent>
{
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    public UserConnectedToTwitterEventHandler(IConfiguration configuration, IEventBus eventBus)
    {
        _eventBus = eventBus;
        _configuration = configuration;
    }
    
    public void Handle(UserConnectedToTwitterEvent e)
    {
        
    }
}