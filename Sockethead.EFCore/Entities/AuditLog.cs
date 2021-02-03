using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sockethead.EFCore.Entities
{
    public class AuditLog
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(256)]
        public string UserName { get; set; }

        [MaxLength(256)]
        public string UserEmail { get; set; }

        public EntityState EntityState { get; set; }

        [Required]
        public string TableName { get; set; }

        //[Required, MaxLength(256)]
        //public string Database { get; set; }

        [Required, MaxLength(256)]
        public string RecordId { get; set; }

        public DateTime TimeStamp { get; set; }

        public virtual ICollection<AuditLogChange> AuditLogChanges { get; set; }
    }
}
