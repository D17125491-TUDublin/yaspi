using yaspi.mvc.Models;
using yaspi.common;

namespace yaspi.mvc.Code;

public class LocalServicesConnectionManager : IConnectionManager
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IEventBus _eventBus;
    public LocalServicesConnectionManager(IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _eventBus = eventBus;
    }

    public void Connect(string username, int connectorId, KeyValuePair<string,string>[] connectionData)
    {
        var c =  new YaspiConnection(){YaspiConnectorId = connectorId,IsActive = true,Username = username,ConnectionData = connectionData};
        AddYaspiConnectionEvent e = new AddYaspiConnectionEvent(c);
        _eventBus.Publish(e);
    }

    public List<ConnectorViewModel> GetActiveConnectorsViewModels()
    {
        ListYaspiConnectorsQuery query = new ListYaspiConnectorsQuery(_connectionString);
        List<YaspiConnector> c  = query.Execute();
        List<ConnectorViewModel> connectors = new List<ConnectorViewModel>();
        foreach (var item in c)
        {
            if(item.IsActive)
                connectors.Add(new ConnectorViewModel(){Name = item.YaspiConnectorName,Base64LargeIcon = item.Base64LargeIcon,Base64SmallIcon = item.Base64SmallIcon,RedirectUrl = item.RedirectUrl});
        }
        return connectors;
    }

    public List<YaspiConnectionViewModel> GetActiveUserConnections(string username)
    {
        throw new NotImplementedException();
    }

    public List<YaspiConnectionViewModel> GetAllUserConnections(string username)
    {
        ListYaspiConnectionsQuery query = new ListYaspiConnectionsQuery(username, _connectionString);
        IEnumerable<YaspiConnection> c  = query.Execute();
        List<YaspiConnectionViewModel> connections = new List<YaspiConnectionViewModel>();
        foreach (var item in c)
        {
            connections.Add(new YaspiConnectionViewModel(){
                Id = item.YaspiConnectionId,
                IsActive = item.IsActive,
                Username = item.Username,
                YaspiConnectorId = item.YaspiConnectorId,
                YaspiConnectorName = item.YaspiConnectorName,
                YaspiConnectionName = item.YaspiConnectionName
            });
        }
        return connections;
    }

    public ConnectorViewModel GetConnectorViewModel(string connectorName)
    {
        GetYaspiConnectorByNameQuery query = new GetYaspiConnectorByNameQuery(connectorName, _connectionString);
        YaspiConnector c  = query.Execute();
        return new ConnectorViewModel(){Name = c.YaspiConnectorName,Base64LargeIcon = c.Base64LargeIcon,Base64SmallIcon = c.Base64SmallIcon,RedirectUrl = c.RedirectUrl,Id = c.YaspiConnectorId};
    }

    public MessagesViewModel GetUserMessagesVewModel(string username)
    {
        var messages = new GetUserMessagesQuery(username, _connectionString).Execute();
        return new MessagesViewModel(){Messages = messages};
    }

    public bool HasActiveConnection(string username)
    {
        ListYaspiConnectionsQuery query = new ListYaspiConnectionsQuery(username, _connectionString);
        IEnumerable<YaspiConnection> c  = query.Execute();
        return c.Any(x => x.IsActive);
    }

    public void PostToAllActiveConnections(string username, string userInput)
    {
        ListYaspiConnectionsQuery query = new ListYaspiConnectionsQuery(username, _connectionString);
        IEnumerable<YaspiConnection> c  = query.Execute();
        foreach (var item in c)
        {
            if(item.IsActive)
            {
                SendYaspiMessageEvent @event = new SendYaspiMessageEvent(new YaspiMessage(){YaspiConnectionId = item.YaspiConnectionId,Text = userInput});
                _eventBus.Publish(@event);
            }
        }
    }

    public void SetActive(string username, int connectionId, bool isActive)
    {
        throw new NotImplementedException();
    }

    public void ToggleUserConnectionActiveById(string name, int connectionId)
    {
        ListYaspiConnectionsQuery query = new ListYaspiConnectionsQuery(name, _connectionString);
        IEnumerable<YaspiConnection> c  = query.Execute();
        var connection = c.FirstOrDefault(x => x.YaspiConnectionId == connectionId);
        if(connection != null)
        {
            SetYaspiConnectionActiveEvent @event = new SetYaspiConnectionActiveEvent(connection.YaspiConnectionId, !connection.IsActive);
            _eventBus.Publish(@event);
        }
    }

    public void SetUserMessageRead(int messageIdToSetAsRead)
    {
        SetUserMessageReadEvent e = new SetUserMessageReadEvent(messageIdToSetAsRead);
        _eventBus.Publish(e);
    }

    public int GetUnreadMessagesCount(string userName)
    {
        return new GetUnreadMessagesCountQuery(userName, _connectionString).Execute();
    }

    public YaspiConnectionViewModel GetConnectionViewModel(int id)
    {
        GetYaspiConnectionByIdQuery query = new GetYaspiConnectionByIdQuery(id, _connectionString);
        YaspiConnection c  = query.Execute();
        return new YaspiConnectionViewModel(){
            Id = c.YaspiConnectionId,
            IsActive = c.IsActive,
            Username = c.Username,
            YaspiConnectorId = c.YaspiConnectorId,
            YaspiConnectorName = c.YaspiConnectorName,
            YaspiConnectionName = c.YaspiConnectionName,
            ConnectionData = c.ConnectionData
        };
    }
}