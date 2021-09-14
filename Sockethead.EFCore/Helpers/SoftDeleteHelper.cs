using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Sockethead.EFCore.Entities;
using System.Linq;

namespace Sockethead.EFCore.Helpers
{
    public static class SoftDeleteHelper
    {
        public static void ProcessSoftDelete(ChangeTracker changeTracker)
        {
            changeTracker.DetectChanges();

            var markedAsDeleted = changeTracker.Entries().Where(x => x.State == EntityState.Deleted);

            foreach (var item in markedAsDeleted)
            {
                if (item.Entity is ISoftDeleteEntity entity)
                {
                    // Set the entity to unchanged (if we mark the whole entity as Modified, every field gets sent to Db as an update)
                    item.State = EntityState.Unchanged;
                    // Only update the IsDeleted flag - only this will get sent to the Db
                    entity.IsDeleted = true;
                }
            }
        }
    }
}
