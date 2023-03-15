namespace yaspi.common;

public class YaspiConnector : IYaspiConnector {
    public int YaspiConnectorId { get; set; }
    public string YaspiConnectorName { get; set; }
    public string RedirectUrl {get;set;}
    public string Base64SmallIcon {get;set;}
    public string Base64LargeIcon {get;set;}
    public string Description {get;set;}
    public bool IsActive {get;set;}

}