using Microsoft.AspNetCore.Mvc;

namespace FluffySpoon.Templates.Sample.Controllers
{
    public class UserController : Controller
    {
        public string GetUsername(int userId)
        {
            return "username" + userId;
        }
    }
}
