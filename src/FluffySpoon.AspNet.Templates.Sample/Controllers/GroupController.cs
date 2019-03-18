using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Templates.Sample.Controllers
{
	public class GroupController : Controller
	{
		[HttpGet("api/groups")]
		public string[] GetAllGroups()
		{
			return new[]
			{
				"foo", "bar"
			};
		}
	}
}
