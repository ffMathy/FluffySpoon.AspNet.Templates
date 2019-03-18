using Microsoft.AspNetCore.Mvc;

namespace FluffySpoon.AspNet.Templates.Sample.Controllers
{
    public class UserController : Controller
	{
		[HttpGet("api/users/{userId}")]
		public string GetUsername(int userId)
        {
            return "username" + userId;
        }
    }
}
