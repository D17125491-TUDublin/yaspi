namespace yaspi.common;

public class YaspiConnectorDataUpdateEvent : IEvent {
    public int YaspiConnectorId { get; set; }
    public KeyValuePair<string,string>[] YaspiConnectorData { get; set; }

    public YaspiConnectorDataUpdateEvent(int yaspiConnectorId, KeyValuePair<string,string>[] connectionData) {
        YaspiConnectorId = yaspiConnectorId;
        YaspiConnectorData = connectionData;
    }
}