using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.Razor.Utilities
{
    // Code from: https://github.com/aspnet/Entropy/blob/dev/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs

    public class RazorViewRenderer : IRazorViewRenderer
    {
        private readonly IRazorViewEngine _viewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;

        public RazorViewRenderer(
            IRazorViewEngine viewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider)
        {
            _viewEngine = viewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Render a Razor view to a string
        /// This is useful for constructing Emails
        /// </summary>
        /// <typeparam name="TModel">Type of the Model the Razor view is expecting</typeparam>
        /// <param name="viewName">Name (and path) of the view to be rendered</param>
        /// <param name="model">Model to pass to the razor view</param>
        /// <param name="viewBag">Optional ViewBag to include</param>
        /// <returns>Fully rendered view</returns>
        public async Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, Dictionary<string, string> viewBag = null)
        {
            var actionContext = GetActionContext();
            var view = FindView(actionContext, viewName);

            var vdd = new ViewDataDictionary<TModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = model,
            };

            if (viewBag != null)
                foreach (var kvp in viewBag)
                    vdd.Add(kvp.Key, kvp.Value);

            using var output = new StringWriter();
            var viewContext = new ViewContext(
                actionContext,
                view,
                vdd,
                new TempDataDictionary(
                    actionContext.HttpContext,
                    _tempDataProvider),
                output,
                new HtmlHelperOptions());

            await view.RenderAsync(viewContext);

            return output.ToString();
        }

        private IView FindView(ActionContext actionContext, string viewName)
        {
            var getViewResult = _viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success)
                return getViewResult.View;

            var findViewResult = _viewEngine.FindView(actionContext, viewName, isMainPage: true);
            if (findViewResult.Success)
                return findViewResult.View;

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                separator: Environment.NewLine,
                values: new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

            throw new InvalidOperationException(errorMessage);
        }

        private ActionContext GetActionContext()
        {
            var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }

    }

    public interface IRazorViewRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model, Dictionary<string, string> viewBag = null);
    }
}
