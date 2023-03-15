namespace yaspi.common;


public class AddYaspiConnectorEvent :IEvent
{
    public IYaspiConnector Connector { get; set; }
    public AddYaspiConnectorEvent(IYaspiConnector connector)
    {
        Connector = connector;
    }
}
