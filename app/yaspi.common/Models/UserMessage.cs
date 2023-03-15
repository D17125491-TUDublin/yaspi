namespace yaspi.common;

public class UserMessage {
    public int UserMessageId {get;set;}
    public string Subject {get;set;}
    public string Body {get;set;}
    public string UserName {get;set;}
    public string SourceName {get;set;}
    public DateTime Created {get;set;}
    public bool IsRead {get;set;}
    public int TypeId {get;set;}

    public static readonly int TYPE_INFO = 1;
    public static readonly int TYPE_WARNING = 2;
    public static readonly int TYPE_ERROR = 3;
}