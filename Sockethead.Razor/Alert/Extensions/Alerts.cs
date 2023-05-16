using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sockethead.Razor.Alert.Extensions
{
    public static class Alerts
    {
        public const string SUCCESS = "success";
        public const string INFORMATION = "info";
        public const string ERROR = "danger";

        public static string[] ALL => new[] { SUCCESS, INFORMATION, ERROR };

        public static void AddMessage(this ITempDataDictionary tempData, string key, string message)
        {
            tempData[key] ??= "";
            tempData[key] += message;
        }

        // Controller Extensions
        public static void Success(this Controller ctrl, string message) => ctrl.TempData.AddMessage(SUCCESS, message);
        public static void Information(this Controller ctrl, string message) => ctrl.TempData.AddMessage(INFORMATION, message);
        public static void Error(this Controller ctrl, string message) => ctrl.TempData.AddMessage(ERROR, message);


        // IActionResult Extensions
        public static IActionResult Success(this IActionResult ar, string message) => new AlertDecoratedResult(ar, SUCCESS, message);
        public static IActionResult Information(this IActionResult ar, string message) => new AlertDecoratedResult(ar, INFORMATION, message);
        public static IActionResult Error(this IActionResult ar, string message) => new AlertDecoratedResult(ar, ERROR, message);

        public class AlertDecoratedResult : IActionResult
        {
            public IActionResult Result { get; }
            public string Type { get; }
            public string Message { get; }

            public AlertDecoratedResult(IActionResult result, string type, string message)
            {
                Result = result;
                Type = type;
                Message = message;
            }

            public async Task ExecuteResultAsync(ActionContext context)
            {
                // Get TempData from the Context and apply the message to it
                context
                    .HttpContext
                    .RequestServices
                    .GetService<ITempDataDictionaryFactory>()
                    .GetTempData(context.HttpContext)
                    .AddMessage(Type, Message);

                // Execute the original IActionResult
                await Result.ExecuteResultAsync(context);
            }
        }
    }
}
