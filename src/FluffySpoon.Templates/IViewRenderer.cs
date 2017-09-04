using System.Threading.Tasks;

namespace FluffySpoon.Templates
{
    public interface IViewRenderer
    {
        Task<string> RenderAsync<TModel>(string name, TModel model);
    }
}