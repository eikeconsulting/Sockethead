using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Sockethead.Web.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.Web.Pages
{
    public class IndexModel : PageModel
    {
        public IndexModel()
        {
        }

        public string Message { get; set; }

        public IActionResult OnGet()
        {
            //Message = $"There are {MyRepo.Db.Users.Count()} users";
            return Redirect("/Samples/Home/Dashboard");
        }
    }
}
