using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Templates
{
    public interface IViewRenderer
    {
        Task<string> RenderAsync<TModel>(string name, TModel model);
    }
}