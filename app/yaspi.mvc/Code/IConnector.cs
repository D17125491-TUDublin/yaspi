namespace yaspi.mvc.Code
{
    public interface IConnector
    {
        public string Id {get;set;}
        public string Name {get;set;}
        public string RedirectUrl {get;set;}
        public IConnectionInfo Connect(string username);
    }
}