using System;
using System.ComponentModel.DataAnnotations;

namespace Sockethead.EFCore.Entities
{
    public class BaseEntity
    {
        [Display(Name = "Revisions")]
        public int Version { get; set; } = 1;

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime Updated { get; set; } = DateTime.UtcNow;

        public void Touch()
        {
            Version++;
            Updated = DateTime.UtcNow;
        }
    }
}
