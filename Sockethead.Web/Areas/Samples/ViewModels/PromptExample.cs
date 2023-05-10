using System.ComponentModel.DataAnnotations;

namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public class PromptExample
    {
        [Display(Prompt = "Enter your name")]
        public string Name { get; set; }
        
        [Display(Prompt = "Enter a description")]
        public string Description { get; set; }
    }
}
