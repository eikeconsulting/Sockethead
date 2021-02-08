using Microsoft.AspNetCore.Mvc;
using Sockethead.Razor.Grid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    public class Movie
    {
        public string Name { get; set; }

        [Display(Name = "Movie Director")]
        public string Director { get; set; }

        [DisplayName("Movie Genre")]
        public string Genre { get; set; }
    }

    [Area("Samples")]
    public class PagerController : Controller
    {
        public List<Movie> _Movies = new List<Movie>
        {
            new Movie { Name = "Star Wars", Director = "George Lucas", Genre = "Sci-Fi" },
            new Movie { Name = "Terminator", Genre = "Action" },
            new Movie { Name = "Wonder Woman", Genre = "Action" },
            new Movie { Name = "Reservoir Dogs", Director="Quentin Tarantino", Genre = "Thriller" },
            new Movie { Name = "Airplane!", Genre = "Slapstick" },
            new Movie { Name = "Close Encounters of the Third Kind", Director = "Steven Spielberg", Genre = "Sci-Fi" },
            new Movie { Name = "Rocky", Director = "John G. Avildsen", Genre = "Action" },
            new Movie { Name = "Brave Heart", Director = "Mel Gibson", Genre = "Action" },
            new Movie { Name = "Movie to be <encoded> html", Director = "This Guy", Genre = "Test" },
        };

        public IActionResult Movies(int page = 1, int sort = 0)
        {
            return View(new SimpleGridViewModel<Movie>
            {
                Source = _Movies.AsQueryable(),
                State = new SimpleGridState
                {
                    PageNum = page,
                    SortColumnId = sort,
                    Search = null,
                    SearchId = 0,
                }
            });
        }
    }
}
