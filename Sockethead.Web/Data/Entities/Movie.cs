using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sockethead.Web.Data.Entities
{
    public class Movie
    {
        public string Name { get; set; } = "";

        [Display(Name = "Movie Director")]
        public string Director { get; set; } = "";

        [DisplayName("Movie Genre")]
        public string Genre { get; set; } = "";

        [Display(AutoGenerateField = true)]
        public int? Released { get; set; }
    }
}
