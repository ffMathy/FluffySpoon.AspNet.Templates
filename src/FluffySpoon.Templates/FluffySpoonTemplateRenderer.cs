using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Linq;

namespace FluffySpoon.Templates
{
    public class FluffySpoonTemplateRenderer
    {
        private readonly IViewRenderer _viewRenderer;
        private readonly IServiceProvider _serviceProvider;

        public FluffySpoonTemplateRenderer(
            IViewRenderer viewRenderer,
            IServiceProvider serviceProvider)
        {
            _viewRenderer = viewRenderer;
            _serviceProvider = serviceProvider;
        }

        private TypeBuilder CreateTypeBuilder(string name)
        {
            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = Guid.NewGuid().ToString();
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(assemblyName), 
                AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName);
            var typeBuilder = moduleBuilder.DefineType(name);
            return typeBuilder;
        }

        public async Task<string> RenderAsync(string name, params Controller[] controllers)
        {
            var viewModelType = GenerateViewModelType(controllers);
            var viewModelProperties = viewModelType
                .GetProperties()
                .Where(x => x.GetMethod.IsPublic);
            var viewModel = Activator.CreateInstance(viewModelType);
            foreach(var viewModelProperty in viewModelProperties)
            {
                viewModelProperty.SetValue(
                    viewModel,
                    _serviceProvider.GetService(viewModelProperty.PropertyType));
            }

            return await _viewRenderer.RenderAsync(name, viewModel);
        }

        private TypeInfo GenerateViewModelType(Controller[] controllers)
        {
            var modelTypeBuilder = CreateTypeBuilder("TemplateViewModel");
            foreach (var controller in controllers)
            {
                var controllerType = controller.GetType();
                var controllerName = controllerType.Name.EndsWith(nameof(Controller)) ?
                    controllerType.Name.Remove(controllerType.Name.LastIndexOf(nameof(Controller))) :
                    controllerType.Name;

                var controllerFieldBuilder = modelTypeBuilder.DefineField(
                    "_" + controllerName,
                    controllerType,
                    FieldAttributes.Private);

                var controllerPropertyGetBuilder = modelTypeBuilder.DefineMethod(
                    "get_" + controllerName,
                    MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
                    controllerType,
                    Type.EmptyTypes);
                var controllerPropertyGetGenerator = controllerPropertyGetBuilder.GetILGenerator();
                controllerPropertyGetGenerator.Emit(OpCodes.Ldarg_0);
                controllerPropertyGetGenerator.Emit(OpCodes.Ldfld, controllerFieldBuilder);
                controllerPropertyGetGenerator.Emit(OpCodes.Ret);

                var controllerPropertyBuilder = modelTypeBuilder.DefineProperty(
                    controllerName,
                    PropertyAttributes.HasDefault,
                    controllerType,
                    null);
                controllerPropertyBuilder.SetGetMethod(controllerPropertyGetBuilder);
            }

            var modelType = modelTypeBuilder.CreateTypeInfo();
            return modelType;
        }
    }
}
