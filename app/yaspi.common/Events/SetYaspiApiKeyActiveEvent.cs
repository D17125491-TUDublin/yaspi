namespace yaspi.common;

public class SetYaspiApiKeyActiveEvent : IEvent {
    public int YaspiApiKeyId { get; set; }
    public bool IsActive { get; set; }
    public SetYaspiApiKeyActiveEvent(int yaspiApiKeyId, bool isActive) {
        YaspiApiKeyId = yaspiApiKeyId;
        IsActive = isActive;
    }
}