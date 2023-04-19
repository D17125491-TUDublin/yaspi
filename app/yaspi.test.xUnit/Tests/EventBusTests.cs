namespace yaspi.test.xUnit;

using Microsoft.Extensions.Configuration;
using yaspi.common;

public class EvetBusTests
{
    private static IEventBusSubscriber GetEventBusSubscriber()
    {
        return new EventBusTestEventBusSubscriber();
    }
    
    private IConfiguration _getConfiguration()
    {
        return ConfigurationFactory.GetConfiguration();
    }

   private IEventBus _getNewEventBus()
    {
        return new EventBus(_getConfiguration(), GetEventBusSubscriber());
    }

    [Fact]
    public void SubscribedHandlerIsHandlingEvent()
    {
        IEventBus eventBus = _getNewEventBus();
        string msg = "Hello World!";
        eventBus.Publish(new TestEvent { Message = msg });
        Xunit.Assert.Equal(msg, TestEventHandler.Message);
    }

    [Fact]
    public void UnsubscribedHandlerIsNotHandlingEvent()
    {
        IEventBus eventBus = _getNewEventBus();
        string msg = "Hello World!";
        eventBus.Publish(new TestEvent { Message = msg });
        Xunit.Assert.Equal(msg, TestEventHandler.Message);

        TestEventHandler.Message = string.Empty;
        eventBus.Unsubscribe<TestEvent, TestEventHandler>();
        eventBus.Publish(new TestEvent { Message = msg });
        Xunit.Assert.NotEqual(msg, TestEventHandler.Message);
    }
}
