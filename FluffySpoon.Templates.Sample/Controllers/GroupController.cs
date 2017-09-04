using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FluffySpoon.Templates.Sample.Controllers
{
    public class GroupController: Controller
    {
        public string[] GetAllGroups()
        {
            return new[]
            {
                "foo", "bar"
            };
        }
    }
}
