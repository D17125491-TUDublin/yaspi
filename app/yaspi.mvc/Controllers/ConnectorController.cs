using System.Net;
using Microsoft.AspNetCore.Mvc;
using yaspi.mvc.Code;
using yaspi.mvc.Models;
using System.Text.Json;
using yaspi.common.OAuth;
using Microsoft.AspNetCore.Authorization;
using yaspi.integration.facebook;
using yaspi.integration.twitter;

namespace yaspi.mvc.Controllers;

[Authorize]
public class ConnectorController : YaspiController
{
    private readonly ILogger<ConnectorController> _logger;
    private readonly IConnectionManager _connectionManagerService;
    private readonly IConfiguration _configuration;
    private readonly IFacebookApiService _facebookApiService;
    private readonly ITwitterApiService _twitterApiService;
    public ConnectorController(ILogger<ConnectorController> logger, IConnectionManager connectionManagerService,
                            IConfiguration configuration, 
                            IFacebookApiService facebookApiService, ITwitterApiService twitterApiService)
    {
        _logger = logger;
        _connectionManagerService = connectionManagerService;
        _configuration = configuration;
        _facebookApiService = facebookApiService;
        _twitterApiService = twitterApiService;
    }

    public IActionResult Index(string connector)
    {

        var value = _connectionManagerService.GetConnectorViewModel(connector);
        return View(value);
    }
    [HttpGet]
    public IActionResult Connect()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Connect(string submit, int connectorId, string connectorName)
    {
        return Redirect("/Home/ListConnectors");
    }


    [HttpGet]
    public IActionResult ConnectTwitter()
    {
        Request.Query.TryGetValue("code", out var code);
        Request.Query.TryGetValue("state", out var stateIn);
        Request.Query.TryGetValue("error", out var error);

        if (error == "access_denied")
        {
            return Redirect("/Home/ListConnectors");
        }
        if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(stateIn)) // initial state
        {
            var url = _twitterApiService.GetAuthRequestUrl();
            return Redirect(url);
        }
        else if (stateIn == "step1")
        {
            var config = _twitterApiService.GetConnectionData(code);
            string username = GetUserName();
            _connectionManagerService.Connect(username, 1, config);
        }
        return View();
    }

    public IActionResult ConnectionSettings(int id)
    {
        YaspiConnectionViewModel value = _connectionManagerService.GetConnectionViewModel(id);
        return View(value);
    }

    [HttpGet]
    public IActionResult ConnectFacebook()
    {
        Request.Query.TryGetValue("code", out var code);
        Request.Query.TryGetValue("state", out var stateIn);
        Request.Query.TryGetValue("error", out var error);
        string client_id = _configuration.GetValue<string>("Facebook:app_id");
        string redirect_uri = _configuration.GetValue<string>("Facebook:redirect_url");
        string token_url = _configuration.GetValue<string>("Facebook:token_url");
        string client_secret = _configuration.GetValue<string>("Facebook:app_secret");

        if (error == "access_denied")
        {
            return Redirect("/Home/ListConnectors");
        }
        if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(stateIn)) // initial state
        {
            string url = _facebookApiService.GetAuthRequestUrl();
            return Redirect(url);
        }
        else if (stateIn == "step1") // 
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            string parameters = $"client_id={client_id}&redirect_uri={redirect_uri}&client_secret={client_secret}&code={code}";
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
            var data = _facebookApiService.GetConnectionData(d.access_token);
            KeyValuePair<string, string>[] tokenInfo = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("access_token", d.access_token),
                new KeyValuePair<string, string>("expires_in", d.expires_in.ToString()),
                new KeyValuePair<string, string>("token_type", d.token_type),
            };
            List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>();
            config.AddRange(tokenInfo);
            config.AddRange(data);
            string username = GetUserName();
            _connectionManagerService.Connect(username, 2, config.ToArray());
        }
        return View();
    }

    [HttpPost]
    public IActionResult ConnectFacebook(string x)
    {
        return View();
    }

    [HttpPost]
    public IActionResult ConnectTwitter(string x)
    {
        HttpRequest request = HttpContext.Request;
        string username = GetUserName();
        return Redirect("/Home/ListConnectors");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Manage()
    {
        List<YaspiConnectionViewModel> connections = _connectionManagerService.GetAllUserConnections(GetUserName());
        var model = new ManageVewModel();
        model.Connections = connections;
        return View(model);
    }

    [HttpPost]
    public IActionResult Manage(ManageVewModel model)
    {
        _connectionManagerService.ToggleUserConnectionActiveById(GetUserName(), model.Toggle);
        model.Connections = _connectionManagerService.GetAllUserConnections(GetUserName());
        return View(model);
    }
}