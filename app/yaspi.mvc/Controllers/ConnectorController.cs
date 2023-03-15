using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using yaspi.mvc.Code;
using yaspi.mvc.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using yaspi.common.OAuth;
using Microsoft.AspNetCore.Authorization;
using yaspi.integration.facebook;

namespace yaspi.mvc.Controllers;

[Authorize]
public class ConnectorController : Controller
{
    private readonly ILogger<ConnectorController> _logger;
    private readonly IConnectionManager _connectionManagerService;
    private readonly IConfiguration _configuration;
    private readonly FacebookApiService _facebookApiService;
    public ConnectorController(ILogger<ConnectorController> logger, IConnectionManager connectionManagerService,
                            IConfiguration configuration, FacebookApiService facebookApiService)
    {
        _logger = logger;
        _connectionManagerService = connectionManagerService;
        _configuration = configuration;
        _facebookApiService = facebookApiService;
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
        string client_id = _configuration.GetValue<string>("Twitter:client_id");
        string redirect_uri = _configuration.GetValue<string>("Twitter:redirect_url");
        string token_url = _configuration.GetValue<string>("Twitter:token_url");
        string scope = _configuration.GetValue<string>("Twitter:scope");
        string code_challenge = "challenge";

        if (error == "access_denied")
        {
            return Redirect("/Home/ListConnectors");
        }
        if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(stateIn)) // initial state
        {
            string stateOut = "step1";
            string code_challenge_method = "plain"; //_configuration.GetValue<string>("Twitter:CodeChallengeMethod");
            string url = $"https://twitter.com/i/oauth2/authorize?response_type=code&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&state={stateOut}&code_challenge={code_challenge}&code_challenge_method={code_challenge_method}";

            return Redirect(url);
        }
        else if (stateIn == "step1")
        {
            // TODO: User pressed 'cancel' button
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
            KeyValuePair<string, string>[] config = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("access_token", d.access_token),
                new KeyValuePair<string, string>("refresh_token", d.refresh_token),
                new KeyValuePair<string, string>("expires_in", d.expires_in.ToString()),
                new KeyValuePair<string, string>("scope", d.scope),
                new KeyValuePair<string, string>("token_type", d.token_type),
            };
            string username = User.Identity.Name;
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
            // TODO: User pressed 'cancel' button
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
            var data = _facebookApiService.GetConnectionData(d);
            KeyValuePair<string, string>[] tokenInfo = new KeyValuePair<string, string>[]
            {
                new KeyValuePair<string, string>("access_token", d.access_token),
                new KeyValuePair<string, string>("expires_in", d.expires_in.ToString()),
                //new KeyValuePair<string, string>("scope", d.scope),
                new KeyValuePair<string, string>("token_type", d.token_type),
            };
            List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>();
            config.AddRange(tokenInfo);
            config.AddRange(data);
            string username = User.Identity.Name;
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
        string username = User.Identity.Name;
        return Redirect("/Home/ListConnectors");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Manage()
    {
        List<YaspiConnectionViewModel> connections = _connectionManagerService.GetAllUserConnections(User.Identity.Name);
        var model = new ManageVewModel();
        model.Connections = connections;
        return View(model);
    }

    [HttpPost]
    public IActionResult Manage(ManageVewModel model)
    {
        _connectionManagerService.ToggleUserConnectionActiveById(User.Identity.Name, model.Toggle);
        model.Connections = _connectionManagerService.GetAllUserConnections(User.Identity.Name);
        return View(model);
    }
}