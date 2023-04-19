namespace yaspi.mvc.Controllers;
    
using Microsoft.AspNetCore.Mvc;
using yaspi.common;

[ApiController]
[Route("api/[controller]")]
public class YaspiMessageController : ControllerBase {

    private readonly IEventBus _eventBus;
    private readonly IConfiguration _configuration;
    private readonly ILogger<YaspiMessageController> _logger;
     public YaspiMessageController(ILogger<YaspiMessageController> logger, IConfiguration configuration, IEventBus eventBus)
    {
        _logger = logger;
        _configuration = configuration;
        _eventBus = eventBus;
    }

    [HttpPost(Name = "PostYaspiMessage")]
    public IActionResult Post()
    {
        Request.Query.TryGetValue("key", out var key);
        Request.Query.TryGetValue("message", out var message);
        Request.Query.TryGetValue("username", out var username);
        if(!string.IsNullOrWhiteSpace(key) && !string.IsNullOrWhiteSpace(message) && !string.IsNullOrWhiteSpace(username))
        {
            GetYaspiApiKeyQuery query = new GetYaspiApiKeyQuery(username,_configuration.GetConnectionString("DefaultConnection"));
            YaspiApiKey apiKey = query.Execute();
            if(apiKey == null || apiKey.Username.ToUpper() != username.ToString().ToUpper() || apiKey.Key != key || !apiKey.IsActive)
            {
                return Unauthorized();
            }
            PostToAllActiveConnectionsEvent _event = new PostToAllActiveConnectionsEvent(message, username);
            _eventBus.Publish(_event);
            return Ok();
        }else{
            return BadRequest();
        }
    }
}