using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using yaspi.mvc.Code;
using yaspi.mvc.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using yaspi.common.OAuth;
using Microsoft.AspNetCore.Authorization;

namespace yaspi.mvc.Controllers;

[Authorize]
public class ConnectionController : Controller {
     private readonly ILogger<ConnectorController> _logger;
    private readonly IConnectionManager _connectionManagerService;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public ConnectionController(ILogger<ConnectorController> logger, IConnectionManager connectionManagerService, IConfiguration configuration) {
        _logger = logger;
        _connectionManagerService = connectionManagerService;
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public IActionResult Settings(int connectionId)
    {
        
        return View();
    }
}