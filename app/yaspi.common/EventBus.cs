namespace yaspi.common;

using Microsoft.Extensions.Configuration;
public class EventBus : IEventBus
{
    Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();
    IConfiguration _config;

    public EventBus(IConfiguration config, IEventBusSubscriber subscriber)
    {
        _config = config;
        subscriber.SubscribeAll(this);
    }
    public void Publish<T>(T _event) where T : IEvent
    {
        if (_handlers.ContainsKey(typeof(T)))
        {
            foreach (var handler in _handlers[typeof(T)])
            {
                ((IEventHandler<T>)handler).Handle(_event);
            }
        }
    }
    public void Subscribe<T, TH>()
        where T : IEvent
        where TH : IEventHandler<T>
    {
        if (!_handlers.ContainsKey(typeof(T)))
        {
            _handlers.Add(typeof(T) , new List<object>());
        }
        var c = typeof(TH).GetConstructor(new Type[] { typeof(IConfiguration), typeof(IEventBus) }); 
        _handlers[typeof(T)].Add((object)c.Invoke(new object[] {(IConfiguration)_config, (IEventBus)this }));
    }
    public void Unsubscribe<T, TH>()
        where T : IEvent
        where TH : IEventHandler<T>
    {
        List<object> toRemove = new List<object>();
        if (_handlers.ContainsKey(typeof(T)))
        {
            foreach(var i in _handlers[typeof(T)])
            {
                if(i.GetType() == typeof(TH))
                {
                    toRemove.Add((object)i);
                }
            }
            foreach(var i in toRemove) _handlers[typeof(T)].Remove(i);
        }
    }
}