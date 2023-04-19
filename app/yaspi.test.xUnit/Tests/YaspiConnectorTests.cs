using yaspi.common;

namespace yaspi.test.xUnit;

public class YaspiConnectorTests  : YaspiTestBase {
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
}