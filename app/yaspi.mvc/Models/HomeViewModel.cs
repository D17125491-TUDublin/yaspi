namespace yaspi.mvc.Models;

public class HomeViewModel {

    
    public bool IsConnected {get;set;}
    public string UserInput  {get;set;}
    public string Action {get;set;}
    public string Message {get;set;}
    public string Submit {get;set;}
    public string ErrorMessage {get;set;}
    public bool HasErrors {get;set;}
    public string SuccessMessage {get;set;}
    public int MessageMaxChar {get;set;}
}