namespace yaspi.common;

public class YaspiConnectionErrorEvent : IEvent {
    public int YaspiConnectionId { get; set; }
    public string ErrorMessage { get; set; }
        
    public YaspiConnectionErrorEvent(int yaspiConnectionId, string message) {
        YaspiConnectionId = yaspiConnectionId;
        ErrorMessage = message;
    }
}