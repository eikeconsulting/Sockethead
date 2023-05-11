using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public enum Gender
    {
        Unknown, Male, Female
    }

    public class UserProfile
    {
        [HiddenInput] public Guid UserId { get; set; } = Guid.NewGuid();

        [DisplayName("First Name")]
        [Required(ErrorMessage = "We need a first name here")]
        [MaxLength(20, ErrorMessage = "First name is too long...")]
        public string First { get; set; } = "John";

        [Display(Name = "Last Name", Order = 2)]
        [MaxLength(20)]
        public string Last { get; set; } = "Doe";

        [Display(Name = "Job Title")] public string JobTitle { get; set; }

        [DisplayName("Administrator")] public bool IsAdmin { get; set; }

        [DisplayName("Nerd")] public bool IsNerd { get; set; } = true;
        
        public Gender Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } = new(year: 1970, month: 4, day: 1);
        
        public decimal Weight { get; set; } = 175.5m;
        
        [ScaffoldColumn(false)] public string City { get; set; } = "SJ";
        [ScaffoldColumn(false)] public string State { get; set; } = "CA";

        [ScaffoldColumn(false)] public string Country { get; set; } = "USA";
        
        [ScaffoldColumn(false)] public string[] Hobbies { get; set; }

        public override string ToString() =>
            $"{UserId}: {First} {Last}, {JobTitle}, Is Admin: {IsAdmin}, Gender: {Gender}";

        public static readonly List<SelectListItem> CityList = new()
        {
            new("Chicago", "CHI"),
            new("Karachi", "KA"),
            new("London", "LDN"),
            new("New York", "NY"),
            new("San Jose", "SJ"),
        };

        public static readonly List<SelectListItem> StateList = new()
        {
            new("California", "CA"),
            new("Florida", "FLA"),
            new("Illinois", "ILL"),
            new("New York", "NY"),
        };
        
        public static readonly List<SelectListItem> CountryList = new()
        {
            new("United States of America", "USA"),
            new("England", "ENG"),
            new("France", "FRA"),
            new("Pakistan", "PAK"),
        };
        
        public static readonly List<SelectListItem> HobbyList = new()
        {
            new("Handball", "Handball"),
            new("Golf", "Golf"),
            new("Tennis", "Tennis"),
            new("Fishing", "Fishing"),
            new("Video Games", "Video Games"),
        };
    }
}