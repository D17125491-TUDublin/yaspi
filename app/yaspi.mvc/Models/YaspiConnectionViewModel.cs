public class YaspiConnectionViewModel{
    public int Id {get;set;}
    public int YaspiConnectorId {get;set;}
    public string YaspiConnectorName {get;set;}
    public string YaspiConnectionName {get;set;}
    public string Username {get;set;}
    public string RedirectUrl {get;set;}
    // public string PublicKeyToken {get;set;}
    public bool IsActive{get;set;}
    public KeyValuePair<string,string>[] ConnectionData {get;set;}
}