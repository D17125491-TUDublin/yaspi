namespace yaspi.common;

public class YaspiMessageSentErrorEvent : IEvent {
    public int MessageId { get; set; }
    public string ErrorMessage { get; set; }
    public YaspiMessageSentErrorEvent(int messageId, string error) {
        MessageId = messageId;
        ErrorMessage = error;
    }
}