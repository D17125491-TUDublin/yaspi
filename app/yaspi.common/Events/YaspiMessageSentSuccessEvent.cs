namespace yaspi.common;

public class YaspiMessageSentSuccessEvent : IEvent {
    public int MessageId { get; set; }
    public string Location {get;set;}
    public YaspiMessageSentSuccessEvent(int messageId, string location) {
        MessageId = messageId;
        Location = location;
    }
}