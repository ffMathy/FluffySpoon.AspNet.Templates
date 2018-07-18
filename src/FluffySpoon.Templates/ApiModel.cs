using System;
using System.Collections.Generic;

public class ApiModel
{
	private readonly Func<string, object> _callback;

	public ApiModel(
		Func<string, object> callback)
	{
		_callback = callback;
	}

	public dynamic Get(string route)
	{
		return Get<dynamic>(route);
	}

	public IEnumerable<dynamic> GetCollection(string route)
	{
		return GetCollection<dynamic>(route);
	}

	public IEnumerable<TApiResult> GetCollection<TApiResult>(string route)
	{
		return Get<IEnumerable<TApiResult>>(route);
	}

	public TApiResult Get<TApiResult>(string route)
	{
		return (TApiResult)_callback(route);
	}
}
