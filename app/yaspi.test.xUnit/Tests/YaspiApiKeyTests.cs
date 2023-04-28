using yaspi.common;

namespace yaspi.test.xUnit;

public class YaspiApiKeyTests : YaspiTestBase {

    [Fact]
    public void GenerateYaspiApiKeyEventAndQueryWorks()
    {
          // Arrange
        var q1 = new GetYaspiApiKeyQuery("test_user", connectionString);
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