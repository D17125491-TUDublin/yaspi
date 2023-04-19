using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Web;
using Microsoft.Extensions.Configuration;
using yaspi.common;
using yaspi.common.OAuth;

namespace yaspi.integration.twitter;

public class TwitterApiService : ITwitterApiService
{

    public int ConnectorId { get { return 1; } }
    private readonly IEventBus _eventBus;
    private readonly IConfiguration _configuration;
    private string client_id;
    private string redirect_uri;
    private string token_url;
    private string tweets_url;
    private string scope;
    private string code_challenge;
    private string code_challenge_method;
    private string authorize_url;
    private string _connectionString;


    public TwitterApiService(IEventBus eventBus, IConfiguration configuration)
    {
        _eventBus = eventBus;
        _configuration = configuration;
        client_id = _configuration["Twitter:client_id"];
        redirect_uri = _configuration["Twitter:redirect_url"];
        token_url = _configuration["Twitter:token_url"];
        tweets_url = _configuration["Twitter:tweets_url"];
        scope = _configuration["Twitter:scope"];
        authorize_url = _configuration["Twitter:authorize_url"];
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        code_challenge = "challenge";
        code_challenge_method = "plain";
    }

    // step 1
    public string GetAuthRequestUrl()
    {
        string stateOut = "step1";
        string url = $"{authorize_url}?response_type=code&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&state={stateOut}&code_challenge={code_challenge}&code_challenge_method={code_challenge_method}";
        return url;
    }

    // step 2
    public KeyValuePair<string, string>[] GetConnectionData(string code)
    {
        List<KeyValuePair<string, string>> data_out = new List<KeyValuePair<string, string>>();
        WebClient wc = new WebClient();
        wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        string stateOut = "step2";
        string parameters = $"code={code}&grant_type=authorization_code&client_id={client_id}&redirect_uri={redirect_uri}&code_verifier={code_challenge}&state={stateOut}";
        string ret = null;
        try { ret = wc.UploadString(token_url, parameters); }
        catch (WebException e)
        {// this can happen when user uses <back> button in browser
            using (var reader = new StreamReader(e.Response.GetResponseStream()))
            {
                string err = reader.ReadToEnd();
            }
        }
        System.Console.WriteLine(ret);
        var d = JsonSerializer.Deserialize<OauthBearer>(ret);
        data_out.Add(new KeyValuePair<string, string>("access_token", d.access_token));
        data_out.Add(new KeyValuePair<string, string>("refresh_token", d.refresh_token));
        data_out.Add(new KeyValuePair<string, string>("expires_in", d.expires_in.ToString()));
        data_out.Add(new KeyValuePair<string, string>("scope", d.scope));
        data_out.Add(new KeyValuePair<string, string>("token_type", d.token_type));
        return data_out.ToArray();
    }

    public void SendMessage(YaspiConnection connection, YaspiMessage message)
    {
        string ret = null;
        string text = HttpUtility.JavaScriptStringEncode(message.Text);
        string body = "{\"text\": \"" + text + "\"}";
        string access_token = connection.ConnectionData.AsQueryable().First(c => c.Key == "access_token").Value;
        WebClient wc = new WebClient();
        wc.Headers.Clear();
        wc.Headers.Add("Authorization", $"Bearer {access_token}");
        wc.Headers.Add("Content-Type", "application/json");
        string response = null;
        try
        {
            response = wc.UploadString(tweets_url, body);
        }
        catch (WebException e)
        {
            string errorMessage = null;
            using (var reader = new StreamReader(e.Response.GetResponseStream()))
            {
                errorMessage = reader.ReadToEnd();
            }
            if (errorMessage == "Unauthorized" || errorMessage.StartsWith("Authenticating "))
            {
                refreshToken(connection, message);
            }
        }
        JsonNode data = JsonSerializer.Deserialize<JsonNode>(response);
        var location = data["data"]["id"].GetValue<string>();
        YaspiMessageSentSuccessEvent messageSentEvent = new YaspiMessageSentSuccessEvent(message.YaspiMessageId, location);
        _eventBus.Publish(messageSentEvent);
    }

    private void refreshToken(YaspiConnection connection, YaspiMessage message)
    {

        string refresh_token = connection.ConnectionData.AsQueryable().First(c => c.Key == "refresh_token").Value;
        string body = $"grant_type=refresh_token&refresh_token={refresh_token}&client_id={client_id}";
        WebClient wc = new WebClient();
        wc.Headers.Clear();
        wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
        string response = null;
        try
        {
            response = wc.UploadString(token_url, body);
        }
        catch (WebException ex)
        {
            var s2 = ex.Response.GetResponseStream();
            var sr2 = new StreamReader(s2);
            var z2 = sr2.ReadToEnd();
            JsonNode data3 = JsonSerializer.Deserialize<JsonNode>(z2);
            string errorMessage2 = data3["error_description"].GetValue<string>();
            YaspiMessageSentErrorEvent messageSentErrorEvent2 = new YaspiMessageSentErrorEvent(message.YaspiMessageId, errorMessage2);
            _eventBus.Publish(messageSentErrorEvent2);
            YaspiConnectionErrorEvent connectionDeletedEvent = new YaspiConnectionErrorEvent(connection.YaspiConnectionId, errorMessage2);
            _eventBus.Publish(connectionDeletedEvent);
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
        YaspiConnectionDataUpdateEvent updateEvent = new YaspiConnectionDataUpdateEvent(connection.YaspiConnectionId, config);
        _eventBus.Publish(updateEvent);
    }
}