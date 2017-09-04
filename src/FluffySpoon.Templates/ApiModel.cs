using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.Templates
{
    public class ApiModel
    {
		private readonly Func<string, object> _callback;

		public ApiModel(
			Func<string, object> callback)
		{
			_callback = callback;
		}

		public dynamic Get(string route) {
			return Get<dynamic>(route);
		}

		public TApiResult Get<TApiResult>(string route) {
			return (TApiResult)_callback(route);
		}
    }
}
