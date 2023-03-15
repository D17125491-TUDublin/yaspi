namespace yaspi.mvc.Models;
public class ConnectorViewModel {
    public string Name {get;set;}
    public string RegisterConnectionRedirectUrl {get;set;}
    // For use in <img> elements:
    public string Base64SmallIcon {get;set;}
    // For use in <img> elements:
    public string Base64LargeIcon {get;set;}
    public string RedirectUrl {get;set;}
    public int Id { get; set; }
}
