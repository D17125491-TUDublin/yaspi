using yaspi.common;

namespace yaspi.test.xUnit;

public class EventBusTestEventBusSubscriber : IEventBusSubscriber {
    public void SubscribeAll(IEventBus eventBus)
    {
       eventBus.Subscribe<TestEvent, TestEventHandler>();
    }
}