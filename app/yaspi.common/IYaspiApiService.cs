using yaspi.common.OAuth;

namespace yaspi.common;

public interface IYaspiApiService
{
    public string GetAuthRequestUrl();
    public KeyValuePair<string, string>[] GetConnectionData(string token);
    public void SendMessage(YaspiConnection connection, YaspiMessage message);
    public int ConnectorId { get; }
}