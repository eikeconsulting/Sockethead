using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sockethead.Web.Areas.Samples.ViewModels
{
    public class FileUploadViewModel
    {
        [Display(Name = "Image File")]
        //[FileExtensions(Extensions = "jpg,jpeg,png,gif")]
        public IFormFile ImageFile { get; set; }
        
        [Display(Name = "Multiple Text Files")]
        //[FileExtensions(Extensions = "txt,csv,log")]
        public List<IFormFile> MultipleTextFiles { get; set; } 

    }
}