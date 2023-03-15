namespace yaspi.common;

public class SetUserMessageReadEvent : IEvent {
    public int UserMessageId { get; set; }
    public SetUserMessageReadEvent(int userMessageId) {
        UserMessageId = userMessageId;
    }
}