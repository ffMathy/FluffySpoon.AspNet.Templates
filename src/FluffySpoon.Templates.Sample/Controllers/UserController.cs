using Microsoft.AspNetCore.Mvc;

namespace FluffySpoon.Templates.Sample.Controllers
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
