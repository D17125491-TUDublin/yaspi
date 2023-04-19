using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using yaspi.mvc.Code;
using yaspi.mvc.Models;
using yaspi.common;

namespace yaspi.mvc.Controllers;

[Authorize]
public class HomeController : YaspiController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConnectionManager _connectionManagerService;
    private readonly IConfiguration _configuration;
    private readonly IEventBus _eventBus;
    private readonly int _messageMaxChar;

    public HomeController(ILogger<HomeController> logger, IConnectionManager connectionManagerService, 
                            IConfiguration configuration, IEventBus eventBus)
    {
        _configuration = configuration;
        _logger = logger;
        _connectionManagerService = connectionManagerService;
        _eventBus = eventBus;
        _messageMaxChar = int.Parse(_configuration["MessageMaxChar"]);
    }

    public IActionResult Index()
    {
        return View(new HomeViewModel() { IsConnected = checkIsConnected(), MessageMaxChar = _messageMaxChar });
    }

    [HttpPost]
    public IActionResult Index(HomeViewModel model)
    {
        model.IsConnected = checkIsConnected();
        model.HasErrors = false;
        model.MessageMaxChar = _messageMaxChar;
        try
        {
            _connectionManagerService.PostToAllActiveConnections(GetUserName(), model.UserInput);
        }
        catch (System.Exception e)
        {
            model.HasErrors = true;
            model.ErrorMessage = "Oops! Something went wrong during sending your message!";
            if (_configuration["Debug"] == "true")
            {
                model.ErrorMessage = "Exception message: " + e.Message + "Stack Trace:" + e.StackTrace;
            }
        }
        if (!model.HasErrors) model.SuccessMessage = "Message sent successfully!";
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult ListConnectors()
    {
        ListYaspiConnectorsQuery query = new ListYaspiConnectorsQuery(_configuration.GetConnectionString("DefaultConnection"));
        var c = query.Execute();
        List<ConnectorViewModel> connectors = _connectionManagerService.GetActiveConnectorsViewModels();
        return View(connectors);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private bool checkIsConnected()
    {
        string username = GetUserName();
        return _connectionManagerService.HasActiveConnection(username);
    }

    public IActionResult Messages()
    {
        MessagesViewModel messages = _connectionManagerService.GetUserMessagesVewModel(GetUserName());
        return View(messages);
    }

    [HttpPost]
    public IActionResult Messages(int messageIdToSetAsRead)
    {
        _connectionManagerService.SetUserMessageRead(messageIdToSetAsRead);
        MessagesViewModel messages = _connectionManagerService.GetUserMessagesVewModel(GetUserName());
        return View(messages);
    }

    [HttpPost]
    public JsonResult MessageRead([FromBody]MarkMessageReadModel data)
    {
        _connectionManagerService.SetUserMessageRead(data.Id);
        return Json(new { Status = "Success" });
    }
    
    public IActionResult GetUnreadMessagesCount()
    {
        int count = _connectionManagerService.GetUnreadMessagesCount(GetUserName());
        return Json(count);
    }

    [HttpGet]
    public IActionResult WebApi()
    {
        string username = GetUserName();
        GetYaspiApiKeyQuery query = new GetYaspiApiKeyQuery(username, _configuration.GetConnectionString("DefaultConnection"));
        var res1 = query.Execute();
        if (res1 == null) return View(null);
        WebApiVewModel model = new WebApiVewModel();
        model.Key = res1.Key;
        model.YaspiApiKeyId = res1.YaspiApiKeyId;
        model.IsActive = res1.IsActive;
        return View(model);
    }

    [HttpPost]
    public IActionResult WebApi(string action) 
    {
        string username = GetUserName();
        GetYaspiApiKeyQuery query = new GetYaspiApiKeyQuery(username, _configuration.GetConnectionString("DefaultConnection"));
        switch (action)
        {
            case "generate":
                GenerateYaspiApiKeyEvent ev1 = new GenerateYaspiApiKeyEvent(username);
                _eventBus.Publish(ev1);
                break;
            case "toggle":
                var res1 = query.Execute();
                if (res1 == null) return Error();
                SetYaspiApiKeyActiveEvent ev = new SetYaspiApiKeyActiveEvent(res1.YaspiApiKeyId, !res1.IsActive);
                _eventBus.Publish(ev);
                break;
            default:
                return Error();
        }
        var res2 = query.Execute();
        WebApiVewModel model = new WebApiVewModel();
        model.Key = res2.Key;
        model.YaspiApiKeyId = res2.YaspiApiKeyId;
        model.IsActive = res2.IsActive;
        return View(model);
    }
}
