using Sockethead.EFCore.AuditLogging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sockethead.Web.Data
{
    public class MyRepo : AuditedRepo<ApplicationDbContext>
    {
        public MyRepo(ApplicationDbContext db, AuditLogger auditLogger)
            : base(db, auditLogger)
        {
        }
    }
}
