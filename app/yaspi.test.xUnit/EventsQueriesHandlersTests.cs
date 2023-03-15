using Microsoft.Extensions.Configuration;
using yaspi.common;

namespace yaspi.test.xUnit;

public class EventsQueriesHandlersTests
{
    private IEventBus eventBus;
    private IConfiguration configuration;
    private string connectionString;
    public EventsQueriesHandlersTests()
    {
        configuration = ConfigurationFactory.GetConfiguration();
        eventBus = new EventBus(configuration, GetEventBusSubscriber());
        connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    private static IEventBusSubscriber GetEventBusSubscriber()
    {
        return new TestEventBusSubscriber();
    }
    [Fact]
    public void HandlerInsertsNewConnectionInfoToDatabase()
    {
        // Arrange
        var yaspiConnection = new YaspiConnection();
        string name = Guid.NewGuid().ToString();
        yaspiConnection.YaspiConnectionName = name;
        yaspiConnection.YaspiConnectorId = 1;
        yaspiConnection.Username = "test";
        // eventBus.Subscribe<AddYaspiConnectionEvent, AddYaspiConnectionEventHandler>(); -- tests failed because subscription was moved to EventBus constructor
        var e = new AddYaspiConnectionEvent(yaspiConnection);
        var q = new ListYaspiConnectionsQuery("test", connectionString);
        // Act
        eventBus.Publish(e);
        // Assert
        var results = q.Execute();
        Assert.True(results.Where(x => x.YaspiConnectionName == name).Count() == 1);
    }

    [Fact]
    public void AddConnectorEventCreatesNewConnectorEntry()
    {
        // Arrange
        var yaspiConnector = new YaspiConnector();
        string name = Guid.NewGuid().ToString();
        yaspiConnector.YaspiConnectorName = name;
        yaspiConnector.Description = "test";
        yaspiConnector.Base64LargeIcon = Assets.Base64LargeIconPlaceholder;
        yaspiConnector.Base64SmallIcon = Assets.Base64SmallIconPlaceholder;
        yaspiConnector.RedirectUrl = "/Connector1";

        //eventBus.Subscribe<AddYaspiConnectorEvent, AddYaspiConnectorEventHadler>();
        var e = new AddYaspiConnectorEvent(yaspiConnector);
        var q = new ListYaspiConnectorsQuery(connectionString);
        // Act
        eventBus.Publish(e);
        // Assert
        var results = q.Execute();
        Assert.True(results.Where(x => x.YaspiConnectorName == name).Count() == 1);
    }

    [Fact]
    public void GetYapiConnectorByNameQueryTest()
    {
        // Arrange
        var yaspiConnector = new YaspiConnector();
        string name = Guid.NewGuid().ToString();
        yaspiConnector.YaspiConnectorName = name;
        yaspiConnector.Description = "test";
        yaspiConnector.Base64LargeIcon = Assets.Base64LargeIconPlaceholder;
        yaspiConnector.Base64SmallIcon = Assets.Base64SmallIconPlaceholder;
        yaspiConnector.RedirectUrl = "/Connector1";
        var q = new GetYaspiConnectorByNameQuery(name, connectionString);
        var e = new AddYaspiConnectorEvent(yaspiConnector);
        eventBus.Publish(e);
        // Act
        var result = q.Execute();
        // Assert
        Assert.True(result.YaspiConnectorName == name);
    }

    [Fact]
    public void SetYaspiConnectionActiveEventHandlerWorks()
    {
        // Arrange
        var yaspiConnection = new YaspiConnection();
        string name = Guid.NewGuid().ToString();
        yaspiConnection.YaspiConnectionName = name;
        yaspiConnection.YaspiConnectorId = 1;
        yaspiConnection.Username = "test";
        var e = new AddYaspiConnectionEvent(yaspiConnection);
        var q = new ListYaspiConnectionsQuery("test", connectionString);
        // Act
        eventBus.Publish(e);
        var results = q.Execute();
        var c = results.Where(x => x.YaspiConnectionName == name).First();
        SetYaspiConnectionActiveEvent e2 = new SetYaspiConnectionActiveEvent(c.YaspiConnectionId, false);
        eventBus.Publish(e2);
        var results2 = q.Execute();
        // Assert
        Assert.True(results2.Where(x => x.YaspiConnectionName == name).First().IsActive == false);
    }

    [Fact]
    public void YaspiConnectorDataEventAndQueryWorks()
    {
        // Arrange 

        KeyValuePair<string, string>[] kvp = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("app_access_token", "test1"),
        };
        int connectorId = 2; // facebook as per init
        var e = new YaspiConnectorDataUpdateEvent(connectorId, kvp);
        var q1 = new GetYaspiConnectorDataByConnectorIdQuery(connectorId, connectionString);

        KeyValuePair<string, string>[] kvp2 = new KeyValuePair<string, string>[]
        {
            new KeyValuePair<string, string>("app_access_token", ""),
        };
        var e2 = new YaspiConnectorDataUpdateEvent(connectorId, kvp2);
        var q2 = new GetYaspiConnectorDataByConnectorIdQuery(connectorId, connectionString);

        // Act1 
        eventBus.Publish(e);
        var result = q1.Execute();
             
        // Assert 1
        Assert.True(result.Where(x => x.Key == "app_access_token").First().Value == "test1");

        // Act2
        eventBus.Publish(e2);
        var result2 = q2.Execute();
        // Assert 2
        Assert.True(result2.Where(x => x.Key == "app_access_token").First().Value == "");
    }

    [Fact]
    public void GenerateYaspiApiKeyEventAndQueryWorks()
    {
        var q1 = new GetYaspiApiKeyQuery("test_user", connectionString);
        // Arrange
        var e = new GenerateYaspiApiKeyEvent("test_user");
        var q2 = new GetYaspiApiKeyQuery("test_user", connectionString);
        // Act
        
        var result1 = q1.Execute();
        eventBus.Publish(e);
        var result2 = q2.Execute();
        // Assert
        Assert.True(result1 != result2);
    }

    [Fact]
    public void SetYaspiApiKeyActiveEventHandlerWorks()
    {
        var e1 = new GenerateYaspiApiKeyEvent("test_user");
        eventBus.Publish(e1);
        var q1 = new GetYaspiApiKeyQuery("test_user", connectionString);
        var res1 = q1.Execute();
        bool t1 = res1.IsActive;
        var e2 = new SetYaspiApiKeyActiveEvent(res1.YaspiApiKeyId, !res1.IsActive);
        eventBus.Publish(e2);
        var q2 = new GetYaspiApiKeyQuery("test_user", connectionString);
        var res2 = q1.Execute();
        bool t2 = res2.IsActive;
        Assert.True(t1 != t2);
    }
}