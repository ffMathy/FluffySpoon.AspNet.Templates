using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Internal;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FluffySpoon.Templates
{
	public class FluffySpoonTemplateRenderer : IFluffySpoonTemplateRenderer
	{
		private readonly IViewRenderer _viewRenderer;
		private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;
		private readonly IActionSelector _actionSelector;
		private readonly IModelBinderProvider _modelBinderProvider;

		public FluffySpoonTemplateRenderer(
			IViewRenderer viewRenderer,
			IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,
			IActionSelector actionSelector,
			IModelBinderProvider modelBinderProvider)
		{
			_viewRenderer = viewRenderer;
			_actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
			_actionSelector = actionSelector;
			_modelBinderProvider = modelBinderProvider;
		}

		public async Task<string> RenderAsync(
			string name,
			params Controller[] controllers)
		{
			var viewModel = new ApiModel(route => CallControllerAction(route, controllers));
			return await _viewRenderer.RenderAsync(name, viewModel);
		}

		private RouteValueDictionary GetRouteValueDefaults(
			RouteTemplate parsedTemplate)
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

		private object CallControllerAction(
			string route,
			Controller[] controllers)
		{
			var actionDescriptors = _actionDescriptorCollectionProvider
				.ActionDescriptors
				.Items
				.OfType<ControllerActionDescriptor>();

			foreach (var actionDescriptor in actionDescriptors) {
				var attributeRouteInformation = actionDescriptor.AttributeRouteInfo;
				if (attributeRouteInformation == null)
					continue;

				var template = TemplateParser.Parse(attributeRouteInformation.Template);
				var templateMatcher = new TemplateMatcher(
					template,
					GetRouteValueDefaults(template));

				var values = new RouteValueDictionary();
				if (!templateMatcher.TryMatch(route, values))
					continue;
					
				var method = actionDescriptor.MethodInfo;
				var controller = controllers.Single(x => x.GetType() == actionDescriptor.ControllerTypeInfo);

				var parameters = method
					.GetParameters()
					.Select(x => values
						.Single(y => y.Key == x.Name)
						.Value)
					.Cast<object>()
					.ToArray();
				return method.Invoke(
					controller,
					parameters);
			}

			throw new InvalidOperationException("No controller to serve the route \"" + route + "\" was found.");
		}
	}
}
