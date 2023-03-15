namespace yaspi.test.xUnit;

using yaspi.common;
public class TestEventBusSubscriber :IEventBusSubscriber {
       public void SubscribeAll(IEventBus eventBus)
    {
        eventBus.Subscribe<AddYaspiConnectorEvent, AddYaspiConnectorEventHadler>();
        eventBus.Subscribe<AddYaspiConnectionEvent, AddYaspiConnectionEventHandler>();
        eventBus.Subscribe<SetYaspiConnectionActiveEvent, SetYaspiConnectionActiveEventHandler>();
        eventBus.Subscribe<SendYaspiMessageEvent, SendYaspiMessageEventHandler>();
     //   eventBus.Subscribe<UserConnectedToTwitterEvent, UserConnectedToTwitterEventHandler>();
        eventBus.Subscribe<YaspiMessageSentSuccessEvent, YaspiMessageSentSuccessEventHandler>();
        eventBus.Subscribe<YaspiMessageSentErrorEvent, YaspiMessageSentErrorEventHandler>();
        eventBus.Subscribe<YaspiConnectionDataUpdateEvent, YaspiConnectionDataUpdateEventHandler>();
        eventBus.Subscribe<YaspiConnectionErrorEvent, YaspiConnectionErrorEventHandler>();
        eventBus.Subscribe<SendUserMessageEvent,SendUserMessageEventHandler>();
        eventBus.Subscribe<SetUserMessageReadEvent,SetUserMessageReadEventHandler>();
        eventBus.Subscribe<YaspiConnectorDataUpdateEvent, YaspiConnectorDataUpdateEventHandler>();
        eventBus.Subscribe<GenerateYaspiApiKeyEvent, GenerateYaspiApiKeyEventHandler>();
        eventBus.Subscribe<SetYaspiApiKeyActiveEvent, SetYaspiApiKeyActiveEventHandler>();
    }
}
    