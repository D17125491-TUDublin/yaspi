﻿namespace yaspi.integration.facebook;

using Microsoft.Extensions.Configuration;
using yaspi.common;
using System.Net;
using System;
// using System.Text.Json;
using yaspi.common.OAuth;
// using Json.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
public class FacebookApiService : IFacebookApiService
{
    private IEventBus _eventBus;
    private string _appId;
    private string _appSecret;
    private string _oauthUrl;
    private string _tokenUrl;
    private string _redirectUrl;
    private string _debugTokenUrl;
    private string _appAccessToken;
    private string _pageAccessToken;
    private string _connectionString;
    private int _facebookConnectorId;
    /*
        1) ask user to allow
        2) get app access token  -- this needs connector settings, not only connection settings. 
            connector initialisation should check if app access token is valid, if not, get a new one
        3) 
    */
    public FacebookApiService(IEventBus eventBus, string appId, string appSecret,
                             string oauthUrl, string tokenUrl, string redirectUrl,
                             string debugTokenUrl, int facebookConnectorId, string connectionString)
    {
        _eventBus = eventBus;
        _appId = appId;
        _appSecret = appSecret;
        _oauthUrl = oauthUrl;
        _tokenUrl = tokenUrl;
        _debugTokenUrl = debugTokenUrl;
        _redirectUrl = redirectUrl;
        _facebookConnectorId = facebookConnectorId;
        _connectionString = connectionString;
        init();
    }

    public FacebookApiService(IEventBus eventBus, IConfiguration configuration)
    {
        _eventBus = eventBus;
        _appId = configuration["Facebook:app_id"];
        _appSecret = configuration["Facebook:app_secret"];
        _oauthUrl = configuration["Facebook:oauth_url"];
        _tokenUrl = configuration["Facebook:token_url"];
        _debugTokenUrl = configuration["Facebook:debug_token_url"];
        _redirectUrl = configuration["Facebook:redirect_url"];
        int.TryParse(configuration["Facebook:connector_id"], out _facebookConnectorId);
        _connectionString = configuration["ConnectionStrings:DefaultConnection"];
        init();
    }

    private void init()
    {
        getAppAccessToken();
    }

    // step 1
    public string GetAuthRequestUrl()
    {
        return $"{_oauthUrl}?response_type=code&client_id={_appId}&redirect_uri={_redirectUrl}&state=step1&scope=pages_manage_posts";
    }

    // step 2
    public KeyValuePair<string,string>[] GetConnectionData(OauthBearer token)
    {
        List<KeyValuePair<string, string>> data_out = new List<KeyValuePair<string, string>>();
        string url1 = $"{_debugTokenUrl}?input_token={token.access_token}&access_token={_appAccessToken}";
        var wc = new WebClient();
        var response = wc.DownloadString(url1);
        dynamic x = JsonConvert.DeserializeObject(response);
        string user_id = x.data.user_id;
        data_out.Add(new KeyValuePair<string, string>("user_id", user_id));
        string url2 = $"https://graph.facebook.com/v16.0/{x.data.user_id}/accounts?access_token={token.access_token}";
        var response2 = wc.DownloadString(url2);
        dynamic y = JsonConvert.DeserializeObject(response2);
        foreach( var z in y.data){
            data_out.Add(new KeyValuePair<string, string>("page_id", (string)z.id));
            data_out.Add(new KeyValuePair<string, string>("page_access_token", (string) z.access_token));
        }
        return data_out.ToArray();
    }

    public string PostToPage(string pageId, string pageAccessToken, string message)
    {
        message = System.Web.HttpUtility.UrlEncode(message);
        var url = $"https://graph.facebook.com/{pageId}/feed?message={message}&access_token={pageAccessToken}";
        var wc = new WebClient();
        var response = wc.UploadString(url,"");
        dynamic x = JsonConvert.DeserializeObject(response);
        return x.id;
    }

    private void getAppAccessToken()
    {
        // https://graph.facebook.com/oauth/access_token?client_id=224761249966998&client_secret=5db0a2a4573852f1292f384b30040e7b&grant_type=client_credentials
        GetYaspiConnectorDataByConnectorIdQuery query = new GetYaspiConnectorDataByConnectorIdQuery(_facebookConnectorId, _connectionString);
        var result = query.Execute();
        foreach (var x in result)
        {
            if (x.Key == "app_access_token")
            {
                if (string.IsNullOrWhiteSpace(x.Value))
                {
                    _appAccessToken = callApiForAppAccessToken();
                    //saveAppAccessToken(); this should not be saved for security reasons
                }
                else _appAccessToken = x.Value;
                return;
            }
        }
        throw new System.Exception("App access token not found");
    }

    private void saveAppAccessToken()
    {
        KeyValuePair<string, string>[] data = new KeyValuePair<string, string>[]
        {
             new KeyValuePair<string, string>("app_access_token", _appAccessToken),
        };
        YaspiConnectorDataUpdateEvent e = new YaspiConnectorDataUpdateEvent(_facebookConnectorId, data);
        _eventBus.Publish(e);
    }

    private string callApiForAppAccessToken()
    {
        var url = $"{_tokenUrl}?client_id={_appId}&client_secret={_appSecret}&grant_type=client_credentials";
        var wc = new WebClient();
        var response = wc.DownloadString(url);
        OauthBearer d = JsonConvert.DeserializeObject<OauthBearer>(response);
        return d.access_token;
    }
}
