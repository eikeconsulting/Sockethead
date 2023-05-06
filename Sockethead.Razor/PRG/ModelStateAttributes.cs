using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Sockethead.Razor.Alert.Extensions;

namespace Sockethead.Razor.PRG
{
    // Inspired by:
    // https://andrewlock.net/post-redirect-get-using-tempdata-in-asp-net-core/
    // https://gist.github.com/Yaevh/e87f682a3c3ac35d1504c068c9f5e8ab

    /// <summary>
    /// Based class for ModelState Attributes
    /// </summary>
    public abstract class ModelStateAttributeBase : ActionFilterAttribute
    {
        protected const string Key = "ModelState";
    }

    /// <summary>
    /// Serialize and save the ModelState to TempData so that a POST can redirect and maintain state.
    /// This Attribute should be applied to the HttpPost controller endpoint.
    /// </summary>
    public class SaveModelStateAttribute : ModelStateAttributeBase
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            if (filterContext.Controller is not Controller controller)
                return;

            var modelState = controller.ViewData.ModelState;
            if (modelState.IsValid)
                return;

            IActionResult result = filterContext.Result;
            if (result is Alerts.AlertDecoratedResult alertResult)
                result = alertResult.Result;
            
            if (result is RedirectResult or RedirectToRouteResult or RedirectToActionResult)
                controller.TempData[Key] = ModelStateHelpers.SerializeModelState(modelState);
        }
    }

    /// <summary>
    /// Restore a serialized ModelState from TempData if present.
    /// This Attribute should be applied to the HttpGet controller endpoint.
    /// </summary>
    public class RestoreModelStateAttribute : ModelStateAttributeBase
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (filterContext.Controller is not Controller controller || !controller.TempData.ContainsKey(Key))
                return;

            /*
            IActionResult result = filterContext.Result;
            if (result is Alerts.AlertDecoratedResult alertResult)
                result = alertResult.Result;
            
            if (result is not ViewResult)
            {
                controller.TempData.Remove(Key);
                return;
            }
            */
             
            string s = controller.TempData[Key].ToString();
            ModelStateDictionary modelState = ModelStateHelpers.DeserializeModelState(s);
            controller.ViewData.ModelState.Merge(modelState);
            controller.TempData.Remove(Key);
        }
    }
}
