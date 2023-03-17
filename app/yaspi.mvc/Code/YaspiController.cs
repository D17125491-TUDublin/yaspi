using Microsoft.AspNetCore.Mvc;

namespace yaspi.mvc;

public class YaspiController : Controller {
    protected string GetUserName() {
        return User.Identity.Name.ToUpper();
    }
}