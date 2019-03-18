using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FluffySpoon.AspNet.Templates
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(
			string name, 
			params Controller[] controllers);
    }
}