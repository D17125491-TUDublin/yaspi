namespace yaspi.common;

public class GenerateYaspiApiKeyEvent : IEvent {
    public string Username { get; set; }
    public GenerateYaspiApiKeyEvent(string username) {
        Username = username;
    }
}