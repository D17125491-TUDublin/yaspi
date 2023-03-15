using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using yaspi.common;
using yaspi.common.OAuth;
using yaspi.common.Queries;

namespace yaspi.mvc.BackgroundServices;

public class TwitterBackgroundService : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private readonly IEventBus _eventBus;
    private readonly string _clientId;
    private readonly string _tokenUrl;
    private readonly string _authorizeUrl;

    public TwitterBackgroundService(IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _eventBus = eventBus;
        _clientId = _configuration["Twitter:client_id"];
        _tokenUrl = _configuration["Twitter:token_url"];
        _authorizeUrl = _configuration["Twitter:authorize_url"];
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Thread.Sleep(5000);
                // TODO: replace ID:1 with something configurable
                GetYaspiMessagesByConnectorIdQuery query = new GetYaspiMessagesByConnectorIdQuery(1, false, false, _connectionString);
                IEnumerable<YaspiMessage> messages = query.Execute();
                System.Console.WriteLine("Twitter Messages to send: " + messages.Count());
                WebClient wc = new WebClient();
                foreach (var item in messages)
                {
                    GetYaspiConnectionByIdQuery query2 = new GetYaspiConnectionByIdQuery(item.YaspiConnectionId, _connectionString);
                    YaspiConnection connection = query2.Execute();
                    // TODO: keep connections info here and reuse, don't query for each message. It requres updating when connection data changes.
                    string url2 = "https://api.twitter.com/2/tweets";

                    string text = HttpUtility.JavaScriptStringEncode(item.Text);
                    string body = "{\"text\": \"" + text + "\"}";
                    string access_token = connection.ConnectionData.AsQueryable().First(c => c.Key == "access_token").Value;
                    wc.Headers.Clear();
                    wc.Headers.Add("Authorization", $"Bearer {access_token}");
                    wc.Headers.Add("Content-Type", "application/json");
                    string x = null;
                    try
                    {
                        x = wc.UploadString(url2, body);
                    }
                    catch (WebException ex)
                    {
                        // "{\"detail\":\"You are not allowed to create a Tweet with duplicate content.\",\"type\":\"about:blank\",\"title\":\"Forbidden\",\"status\":403}"
                        var s = ex.Response.GetResponseStream();
                        var sr = new StreamReader(s);
                        var z = sr.ReadToEnd();
                        // "{\n  \"title\": \"Unauthorized\",\n  \"type\": \"about:blank\",\n  \"status\": 401,\n  \"detail\": \"Unauthorized\"\n}"
                        JsonNode data2 = JsonSerializer.Deserialize<JsonNode>(z);
                        string errorMessage = data2["detail"].GetValue<string>();
                        // second condition added for debugging purposes
                        if (errorMessage == "Unauthorized" || errorMessage.StartsWith("Authenticating ")) 
                        {
                            string refresh_token = connection.ConnectionData.AsQueryable().First(c => c.Key == "refresh_token").Value;
                            string body2 = $"grant_type=refresh_token&refresh_token={refresh_token}&client_id={_clientId}";
                            wc.Headers.Clear();
                            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                            string response = null;
                            try
                            {
                                response = wc.UploadString(_tokenUrl, body2);
                            }
                            catch (WebException ex2)
                            {
                                var s2 = ex2.Response.GetResponseStream();
                                var sr2 = new StreamReader(s2);
                                var z2 = sr2.ReadToEnd();
                                JsonNode data3 = JsonSerializer.Deserialize<JsonNode>(z2);
                                string errorMessage2 = data3["error_description"].GetValue<string>();
                                YaspiMessageSentErrorEvent messageSentErrorEvent2 = new YaspiMessageSentErrorEvent(item.YaspiMessageId, errorMessage2);
                                _eventBus.Publish(messageSentErrorEvent2);
                                YaspiConnectionErrorEvent connectionDeletedEvent = new YaspiConnectionErrorEvent(item.YaspiConnectionId, errorMessage2);
                                _eventBus.Publish(connectionDeletedEvent);
                                continue;
                            }
                            var d = JsonSerializer.Deserialize<OauthBearer>(response);
                            KeyValuePair<string, string>[] config = new KeyValuePair<string, string>[]
                            {
                                    new KeyValuePair<string, string>("access_token", d.access_token),
                                    new KeyValuePair<string, string>("refresh_token", d.refresh_token),
                                    new KeyValuePair<string, string>("expires_in", d.expires_in.ToString()),
                                    new KeyValuePair<string, string>("scope", d.scope),
                                    new KeyValuePair<string, string>("token_type", d.token_type),
                            };
                            YaspiConnectionDataUpdateEvent updateEvent = new YaspiConnectionDataUpdateEvent(item.YaspiConnectionId, config);
                            _eventBus.Publish(updateEvent);
                            continue;
                        }
                        YaspiMessageSentErrorEvent messageSentErrorEvent = new YaspiMessageSentErrorEvent(item.YaspiMessageId, errorMessage);
                        _eventBus.Publish(messageSentErrorEvent);
                        continue;
                    }

                    JsonNode data = JsonSerializer.Deserialize<JsonNode>(x);
                    var id = data["data"]["id"].GetValue<string>();
                    YaspiMessageSentSuccessEvent messageSentEvent = new YaspiMessageSentSuccessEvent(item.YaspiMessageId, id);
                    _eventBus.Publish(messageSentEvent);
                }
            }
        });
        return Task.CompletedTask;
    }
}