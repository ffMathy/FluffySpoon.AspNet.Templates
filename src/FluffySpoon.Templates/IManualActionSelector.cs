using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace FluffySpoon.Templates
{
	public interface IManualActionSelector
	{
		ActionDescriptor GetMatchingAction(string path, params Controller[] controllers);
	}
}