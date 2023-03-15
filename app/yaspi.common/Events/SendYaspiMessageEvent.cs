namespace yaspi.common;

public class SendYaspiMessageEvent : IEvent {
    public YaspiMessage Message { get; set; }
    public SendYaspiMessageEvent(YaspiMessage message) {
        Message = message;
    }
}
