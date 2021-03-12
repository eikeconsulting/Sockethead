using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Sockethead.EFCore.Extensions
{
    public static class EntityExtensions
    {
        /// <summary>
        /// Get the Table() attribute, if one exists
        /// Otherwise return the entity name 
        /// Should handle pluralized name
        /// </summary>
        public static string GetTableName(this EntityEntry dbEntry)
        {
            Type type = dbEntry.Entity.GetType();

            return type
                .GetCustomAttributes(typeof(TableAttribute), false)
                .SingleOrDefault()
                    is TableAttribute attribute
                        ? attribute.Name
                        : type.Name;
        }

        /// <summary>
        /// Returns a string representing the Primary key
        /// If the PK is composite, it will combine the components and separate with a dash
        /// </summary>
        public static string GetPrimaryKeyValue(this EntityEntry dbEntry)
        {
            try
            {
                IKey pk = dbEntry.Metadata.FindPrimaryKey();

                if (pk == null)
                    return "N/A";

                return string.Join("-", pk
                    .Properties
                    .Select(p => dbEntry
                        .Property(p.Name)
                        .CurrentValue
                        .ToString()));
            }
            catch //(Exception ex)
            {
                //Logger.LogError(ex, $"Error retriving recordId through primary key for entity of type: [{dbEntry.GetType().Name}].");
                return "[UNKNOWN-PK]";
            }
        }
    }
}
