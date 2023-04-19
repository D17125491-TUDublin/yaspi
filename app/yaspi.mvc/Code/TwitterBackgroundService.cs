using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using yaspi.common;
using yaspi.common.OAuth;
using yaspi.common.Queries;
using yaspi.integration.twitter;

namespace yaspi.mvc.BackgroundServices;

public class TwitterBackgroundService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IEventBus _eventBus;
    private readonly string _authorizeUrl;
    private readonly ILogger<TwitterBackgroundService> _logger;
    private readonly ITwitterApiService _twitterApiService;

    public TwitterBackgroundService(IConfiguration configuration, IEventBus eventBus, 
            ILogger<TwitterBackgroundService> logger,ITwitterApiService twitterBackgroundService)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _eventBus = eventBus;
        _logger = logger;
        _twitterApiService = twitterBackgroundService;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(5000);
                var query = new GetYaspiMessagesByConnectorIdQuery(_twitterApiService.ConnectorId, false, false, _connectionString);
                IEnumerable<YaspiMessage> messages = query.Execute();
                System.Console.WriteLine("Twitter Messages to send: " + messages.Count());
                WebClient wc = new WebClient();
                foreach (var item in messages)
                {
                    GetYaspiConnectionByIdQuery query2 = new GetYaspiConnectionByIdQuery(item.YaspiConnectionId, _connectionString);
                    YaspiConnection connection = query2.Execute();
                    _twitterApiService.SendMessage(connection, item);
                }
            }
        });
        return Task.CompletedTask;
    }
}