using FluffySpoon.AspNet.Templates.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Templates
{
    class ViewRenderer : IViewRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IViewValidator _viewValidator;

        public ViewRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IViewValidator viewValidator)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _viewValidator = viewValidator;
        }

        public async Task<string> RenderAsync<TModel>(string name, TModel model)
        {
            var actionContext = GetActionContext();
			
            var viewEngineResult = _viewEngine.FindView(actionContext, name, false);
            if (!viewEngineResult.Success)
                throw new InvalidOperationException(string.Format("Couldn't find view '{0}'", name));

            var view = viewEngineResult.View;
            var validationException = await _viewValidator.ValidateAsync(view.Path);
            if (validationException != null)
                throw validationException;

            using (var output = new StringWriter())
            {
                var viewContext = new ViewContext(
                    actionContext,
                    view,
                    new ViewDataDictionary<TModel>(
                        metadataProvider: new EmptyModelMetadataProvider(),
                        modelState: new ModelStateDictionary())
                    {
                        Model = model
                    },
                    new TempDataDictionary(
                        actionContext.HttpContext,
                        _tempDataProvider),
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);
                return output.ToString();
            }
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };

            return new ActionContext(
                httpContext, 
                new RouteData(),
                new ActionDescriptor());
        }
    }
}
