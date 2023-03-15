namespace yaspi.common;

public class PostToAllActiveConnectionsEvent : IEvent
{
    public PostToAllActiveConnectionsEvent(string message,string username)
    {
        Message = message;
        Username = username;
    }

    public string Message { get; }
    public string Username { get; }
}