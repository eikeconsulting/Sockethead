using Sockethead.Web.Data.Entities;
using System.Collections.Generic;

namespace Sockethead.Web.Data
{
    public class SampleData
    {
        public static List<Movie> Movies { get; } = new List<Movie>
        {
            new Movie { Name = "Star Wars", Director = "George Lucas", Genre = "Sci-Fi", Released = 1997, 
                Cast = new List<CastMember>
                {
                    new CastMember { Name = "Mark Hamill", Character = "Luke Skywalker" },
                    new CastMember { Name = "Harrison Ford", Character = "Han Solo" },
                    new CastMember { Name = "Carrie Fisher", Character = "Princess Leia Organa" },
                    new CastMember { Name = "Alec Guinness", Character = "Ben Obi-Wan Kenobi" },
                    new CastMember { Name = "Anthony Daniels", Character = "C-3PO" },
                    new CastMember { Name = "Kenny Baker", Character = "R2-D2" },
                    new CastMember { Name = "Peter Mayhew", Character = "Chewbacca" },
                    new CastMember { Name = "David Prowse", Character = "Darth Vader" },
                }
            },
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

        public static List<SampleModel> SampleModels { get; }  = new List<SampleModel>
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
    }
}
