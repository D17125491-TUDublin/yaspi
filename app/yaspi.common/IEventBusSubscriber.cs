namespace yaspi.common;

public interface IEventBusSubscriber
{
    public void SubscribeAll(IEventBus eventBus);
}