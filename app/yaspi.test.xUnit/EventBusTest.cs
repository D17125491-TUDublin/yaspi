namespace yaspi.test.xUnit;

using Microsoft.Extensions.Configuration;
using yaspi.common;

public class EvetBusTest {
    
    private IEventBus eventBus;

    public EvetBusTest() {

        var bus = new EventBus( _getConfiguration(),GetEventBusSubscriber());
    }
 private static IEventBusSubscriber GetEventBusSubscriber()
    {
        return new TestEventBusSubscriber();
    }
    private IConfiguration _getConfiguration()
    {
       return ConfigurationFactory.GetConfiguration();
    }

    // [Fact]
    // public void RegisteredHandlerIsHandlingEvent() {
    //     string msg = "Hello World!";
    //     eventBus.Subscribe<TestEvent, TestEventHandler>();
    //     eventBus.Publish(new TestEvent { Message = msg });
    //     Xunit.Assert.Equal(msg, TestEventHandler.Message);
    // }

    // [Fact]
    // public void UnregisteredHandlerIsNotHandlingEvent() {
    //     string msg = "Hello World!";
    //     eventBus.Subscribe<TestEvent, TestEventHandler>();
    //     eventBus.Publish(new TestEvent { Message = msg });
    //     Xunit.Assert.Equal(msg, TestEventHandler.Message);

    //     TestEventHandler.Message = string.Empty;
    //     eventBus.Unsubscribe<TestEvent, TestEventHandler>();
    //     eventBus.Publish(new TestEvent { Message = msg });
    //     Xunit.Assert.NotEqual(msg, TestEventHandler.Message);
    // }
}
