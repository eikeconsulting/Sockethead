using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sockethead.EFCore.Entities
{
    public class AuditLogChange
    {
        [Key]
        public long Id { get; set; }

        public long AuditLogId { get; set; }

        [ForeignKey("AuditLogId")]
        public virtual AuditLog AuditLog { get; set; }

        [Required]
        public string Property { get; set; }

        public string Original { get; set; }

        public string Current { get; set; }

    }
}
