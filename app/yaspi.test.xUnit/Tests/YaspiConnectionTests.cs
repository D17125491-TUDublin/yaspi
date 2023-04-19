using yaspi.common;

namespace yaspi.test.xUnit;

public class YaspiConnectionTest : YaspiTestBase {
    
    [Fact]
    public void AddYaspiConnectionEventIsHandlerProperly()
    {
        // Arrange
        string username = Assets.UserName_A;
        var yaspiConnection = new YaspiConnection();
        string name = Guid.NewGuid().ToString();
        yaspiConnection.YaspiConnectionName = name;
        yaspiConnection.YaspiConnectorId = 1;
        yaspiConnection.Username = username;
        var e = new AddYaspiConnectionEvent(yaspiConnection);
        var q = new ListYaspiConnectionsQuery(username, connectionString);
        // Act
        eventBus.Publish(e);
        // Assert
        var results = q.Execute();
        Assert.True(results.Where(x => x.YaspiConnectionName == name).Count() == 1);
    }

 [Fact]
    public void SetYaspiConnectionActiveEventHandlerWorks()
    {
        // Arrange
        string username = Assets.UserName_A;
        var yaspiConnection = new YaspiConnection();
        string name = Guid.NewGuid().ToString();
        yaspiConnection.YaspiConnectionName = name;
        yaspiConnection.YaspiConnectorId = 1;
        yaspiConnection.Username = username;
        var e = new AddYaspiConnectionEvent(yaspiConnection);
        var q = new ListYaspiConnectionsQuery(username, connectionString);
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
}