using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluffySpoon.Templates
{
    public class ManualActionSelector : IManualActionSelector
	{
		private readonly IActionSelector _actionSelector;
		private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

		public ManualActionSelector(
			IActionSelector actionSelector, 
			IActionDescriptorCollectionProvider actionDescriptorCollectionProvider)
		{
			_actionSelector = actionSelector;
			_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
		}

		public ActionDescriptor GetMatchingAction(
			string path, 
			params Controller[] controllers)
		{
			var attributes = controllers
				.Select(x => x
					.GetType())

			var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors.Items;
			
			var matchingDescriptors = new List<ActionDescriptor>();
			foreach (var actionDescriptor in actionDescriptors)
			{
				var matchesRouteTemplate = MatchesTemplate(actionDescriptor.AttributeRouteInfo.Template, path);
				if (matchesRouteTemplate)
				{
					matchingDescriptors.Add(actionDescriptor);
				}
			}
			
			var httpContext = new DefaultHttpContext();
			httpContext.Request.Path = path;
			httpContext.Request.Method = "GET";

			var routeContext = new RouteContext(httpContext);
			return _actionSelector.SelectBestCandidate(routeContext, matchingDescriptors.AsReadOnly());
		}
		
		private bool MatchesTemplate(string routeTemplate, string requestPath)
		{
			var template = TemplateParser.Parse(routeTemplate);

			var matcher = new TemplateMatcher(template, GetDefaults(template));
			var values = new RouteValueDictionary();
			return matcher.TryMatch(requestPath, values);
		}
		
		private RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
		{
			var result = new RouteValueDictionary();

			foreach (var parameter in parsedTemplate.Parameters)
			{
				if (parameter.DefaultValue != null)
				{
					result.Add(parameter.Name, parameter.DefaultValue);
				}
			}

			return result;
		}
	}
}
