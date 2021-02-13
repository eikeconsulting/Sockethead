using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sockethead.Web.Areas.Samples.Controllers
{
    public class Movie
    {
        public string Name { get; set; } = "";

        [Display(Name = "Movie Director")]
        public string Director { get; set; } = "";

        [DisplayName("Movie Genre")]
        public string Genre { get; set; } = "";

        public int? Released { get; set; }
    }

    public enum SampleEnum
    {
        Zero, One, Two, Three, Four
    }

    public class SampleModel
    {
        [Display(Name = "Unique ID")]
        public int Id { get; set; } = NextId++;

        [DisplayName("First Name")]
        public string First { get; set; }

        [Display(Name = "Last Name")]
        public string Last { get; set; }

        public string JobTitle { get; set; }

        public DateTime RandomDate { get; set; } = GetRandomDate();

        public bool Flag { get; set; } = GetRandomBool();

        public SampleEnum SampleEnum { get; set; } = GetRandomEnum();


        private static int NextId = 1001;

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private static DateTime GetRandomDate()
            => DateTime.UtcNow
                .AddDays(-Random.NextDouble() * 10000)
                .AddMinutes(Random.NextDouble() * 24 * 60);

        private static bool GetRandomBool() => Random.NextDouble() > 0.5;

        private static SampleEnum GetRandomEnum() => (SampleEnum)Random.Next((int)SampleEnum.Zero, (int)SampleEnum.Four + 1);

        public override string ToString() => $"Sample Data for {First} {Last}, {JobTitle}";
    }

    [Area("Samples")]
    public class PagerController : Controller
    {
        private static readonly List<Movie> _Movies = new List<Movie>
        {
            new Movie { Name = "Star Wars", Director = "George Lucas", Genre = "Sci-Fi", Released = 1997 },
            new Movie { Name = "Terminator", Genre = "Action", Director = "James Cameron", Released = 1984 },
            new Movie { Name = "Terminator 2: Judgement Day", Genre = "Action", Director = "James Cameron", Released = 1991 },
            new Movie { Name = "Wonder Woman 1984", Genre = "Action", Director = "", Released = 2020 },
            new Movie { Name = "Reservoir Dogs", Director="Quentin Tarantino", Genre = "Thriller", Released = 1992 },
            new Movie { Name = "Airplane!", Genre = "Slapstick", Director="David and Jerry Zucker and Jim Abrahams", Released = 1980 },
            new Movie { Name = "Close Encounters of the Third Kind", Director = "Steven Spielberg", Genre = "Sci-Fi", Released = 1997 },
            new Movie { Name = "Rocky", Director = "John G. Avildsen", Genre = "Action" },
            new Movie { Name = "Brave Heart", Director = "Mel Gibson", Genre = "Action" },
            new Movie { Name = "Movie with <b>bold</b>", Director = "This Guy", Genre = "Test" },
            new Movie { Name = "The Godfather", Director = "The Godfather", Genre = "Mob", Released = 1972 },
            new Movie { Name = "Citizen Kane", Director = "Orson Welles", Genre = "Drama", Released = 1941 },
            new Movie { Name = "The Shawshank Redemption", Director = "Frank Drabont", Genre = "Drama", Released = 1994 },
            new Movie { Name = "Pulp Fiction", Director = "Quentin Tarantino", Genre = "Action", Released = 1994 },
            new Movie { Name = "Casablanca", Director = "Michael Curtiz", Genre = "Drama", Released = 1942 },
            new Movie { Name = "2001: A Space Odyssey", Director = "Stanley Kubrick", Genre = "Sci-Fi", Released = 1968 },
        };

        private static readonly List<SampleModel> _SampleData = new List<SampleModel>
        {
            new SampleModel { First = "John", Last = "Smith", JobTitle = "President" },
            new SampleModel { First = "Jane", Last = "Smith", JobTitle = "CEO" },
            new SampleModel { First = "Jim", Last = "Smith", JobTitle = "Secretary" },
            new SampleModel { First = "Aaron", Last = "Green", JobTitle = "Engineer" },
            new SampleModel { First = "Aaron", Last = "Smith", JobTitle = "Trainer" },
            new SampleModel { First = "Sally", Last = "Green", JobTitle = "Clerk" },
            new SampleModel { First = "Francis", Last = "Jones", JobTitle = "Engineer" },
            new SampleModel { First = "Frank", Last = "Jones", JobTitle = "Engineer" },
            new SampleModel { First = "Mike", Last = "Block", JobTitle = "Helper" },
            new SampleModel { First = "Boomer", Last = "Blow", JobTitle = "Director of Operations" },
        };

        [HttpGet]
        public IActionResult Movies1()
        {
            ViewData["Title"] = "Movies!";
            return View(_Movies.AsQueryable());
        }

        [HttpGet]
        public IActionResult Movies2()
        {
            ViewData["Title"] = "Movies!";
            return View(_Movies.AsQueryable());
        }

        [HttpGet]
        public IActionResult Sample1()
        {
            ViewData["Title"] = "Sample Data Example";
            return View(_SampleData.AsQueryable());
        }

        [HttpGet]
        public IActionResult Sample2()
        {
            ViewData["Title"] = "Sample Data Example";
            return View(_SampleData.AsQueryable());
        }

    }
}
