using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluffySpoon.AspNet.Templates
{
    public static class RegistrationExtensions
    {
        public static void AddFluffySpoonTemplating(this IServiceCollection services)
        {
            services.AddScoped<IViewRenderer, ViewRenderer>();
            services.AddScoped<IViewValidator, ViewValidator>();
            services.AddScoped<ITemplateRenderer, TemplateRenderer>();
        }
    }
}
