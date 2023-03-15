namespace yaspi.common;

public class YaspiMessage
{
    public int YaspiMessageId { get; set; }
    public int YaspiConnectionId { get; set; }
    public string YaspiConnectionName { get; set; }
    public int YaspiConnectorId { get; set; }
    public string Text { get; set; } // TODO:  Twitter max 280 chars - this should be validated on input
    public bool IsSent { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public string Location { get; set; }
    public bool HasError { get; set; }
    public string ErrorMessage { get; set; }
}