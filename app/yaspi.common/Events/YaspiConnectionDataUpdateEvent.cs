namespace yaspi.common;

public class YaspiConnectionDataUpdateEvent : IEvent {
    public int YaspiConnectionId { get; set; }
    public KeyValuePair<string,string>[] YaspiConnectionData { get; set; }

    public YaspiConnectionDataUpdateEvent(int yaspiConnectionId, KeyValuePair<string,string>[] connectionData) {
        YaspiConnectionId = yaspiConnectionId;
        YaspiConnectionData = connectionData;
    }
}