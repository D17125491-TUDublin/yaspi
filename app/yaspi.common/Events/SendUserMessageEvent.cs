namespace yaspi.common;

public class SendUserMessageEvent : IEvent {
    public int YaspiConnectionId { get; set; }
    public UserMessage Message { get; set; }
    public SendUserMessageEvent(int yaspiConnectionId, UserMessage message) {
        YaspiConnectionId = yaspiConnectionId;
        Message = message;
    }
}