using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using yaspi.common;
using yaspi.common.OAuth;
using yaspi.common.Queries;
using yaspi.integration.facebook;

namespace yaspi.mvc.BackgroundServices;
public class FacebookBackgroundService : BackgroundService
{

    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IEventBus _eventBus;
    private readonly IFacebookApiService _facebookApiService;
    private readonly ILogger<FacebookBackgroundService> _logger;

    public FacebookBackgroundService(ILogger<FacebookBackgroundService> logger, IConfiguration configuration, 
                                            IEventBus eventBus, IFacebookApiService facebookApiService)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _eventBus = eventBus;
        _facebookApiService = facebookApiService;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(5000);
                GetYaspiMessagesByConnectorIdQuery query = new GetYaspiMessagesByConnectorIdQuery(_facebookApiService.ConnectorId, false, false, _connectionString);
                IEnumerable<YaspiMessage> messages = query.Execute();
                System.Console.WriteLine("Facebook Messages to send: " + messages.Count());
                foreach (var item in messages)
                {
                    GetYaspiConnectionByIdQuery query2 = new GetYaspiConnectionByIdQuery(item.YaspiConnectionId, _connectionString);
                    YaspiConnection connection = query2.Execute();
                    _facebookApiService.SendMessage(connection, item);
                }
            }
        });
        return Task.CompletedTask;
    }
}
