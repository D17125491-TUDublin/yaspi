namespace yaspi.common;

public class YaspiConnection 
{
    public int YaspiConnectionId { get; set; }
    public int YaspiConnectorId { get; set; }
    public string YaspiConnectionName { get; set; }
    public string YaspiConnectorName { get; set; }
    public string Username { get; set; }
    public bool IsActive { get; set; }
    public KeyValuePair<string, string>[] ConnectionData { get; set; }
}