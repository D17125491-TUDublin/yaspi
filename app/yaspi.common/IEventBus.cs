namespace yaspi.common;

public interface IEventBus
{
    void Publish<T>(T _event) where T : IEvent;
    void Subscribe<T, TH>()
        where T : IEvent
        where TH : IEventHandler<T>;
    void Unsubscribe<T, TH>()
        where T : IEvent
        where TH : IEventHandler<T>;

}
