namespace yaspi.common;
public interface IEventHandler<in TEvent> where TEvent : IEvent
{
    void Handle(TEvent _event);
}
