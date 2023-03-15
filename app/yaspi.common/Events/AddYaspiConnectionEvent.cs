namespace yaspi.common;

public class AddYaspiConnectionEvent : IEvent {
    public YaspiConnection Connection { get; set; }
    public AddYaspiConnectionEvent(YaspiConnection connection) {
        Connection = connection;
    }
}
