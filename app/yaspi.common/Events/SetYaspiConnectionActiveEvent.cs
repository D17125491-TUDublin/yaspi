namespace yaspi.common;

public class SetYaspiConnectionActiveEvent : IEvent {
    public int YaspiConnectionId { get; set; }
    public bool IsActive { get; set; }
    public SetYaspiConnectionActiveEvent(int yaspiConnectionId, bool isActive) {
        YaspiConnectionId = yaspiConnectionId;
        IsActive = isActive;
    }
}
