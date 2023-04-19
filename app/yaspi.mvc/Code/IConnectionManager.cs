using yaspi.mvc.Models;

namespace yaspi.mvc.Code;

public interface IConnectionManager
{
    void Connect(string username, int connectorId, KeyValuePair<string,string>[] connectionData);
    List<YaspiConnectionViewModel> GetAllUserConnections(string username);
    List<ConnectorViewModel> GetActiveConnectorsViewModels();
    ConnectorViewModel GetConnectorViewModel(string connector);
    bool HasActiveConnection(string username);
    void PostToAllActiveConnections(string username, string userInput); 
    void ToggleUserConnectionActiveById(string username, int connectionId);
    MessagesViewModel GetUserMessagesVewModel(string username);
    void SetUserMessageRead(int messageIdToSetAsRead);
    int GetUnreadMessagesCount(string name);
    YaspiConnectionViewModel GetConnectionViewModel(int id);
}