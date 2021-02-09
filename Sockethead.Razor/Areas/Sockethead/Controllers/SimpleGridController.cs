using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Grid;

namespace Sockethead.Razor.Areas.Sockethead.Controllers
{
    [Area("Sockethead")]
    public class SimpleGridController : Controller
    {
        public IActionResult Ping() => Content("Hello World!");

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult SearchHandler(SimpleGridSearchViewModel vm)
            => Redirect(SimpleGridState.AppendSearchParameters(vm.RedirectUrl, vm.Query, vm.SearchNdx));
    }
}
