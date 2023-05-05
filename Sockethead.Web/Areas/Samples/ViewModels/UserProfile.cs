using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public enum Gender
    {
        Unknown, Male, Female
    }

    public class UserProfile
    {
        public Guid UserId { get; set; }
        
        [Display(Order = 3)]
        [DisplayName("First Name")]
        [Required(ErrorMessage = "We need a first name here")]
        [MaxLength(20, ErrorMessage = "First name is too long...")]
        public string First { get; set; }

        [Display(Name = "Last Name", Order = 2)]
        [MaxLength(20)]
        public string Last { get; set; }

        [Display(Name = "Job Title")]
        public string JobTitle { get; set; }
        
        [DisplayName("Administrator")]
        public bool IsAdmin { get; set; }

        public Gender Gender { get; set; }

        public override string ToString() => $"{UserId}: {First} {Last}, {JobTitle}, Is Admin: {IsAdmin}, Gender: {Gender}";
    }
}