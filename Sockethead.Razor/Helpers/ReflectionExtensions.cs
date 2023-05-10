using System;

namespace Sockethead.Razor.Helpers
{
    /// <summary>
    /// https://stackoverflow.com/a/5182747/910348
    /// </summary>
    public static class ReflectionExtensions
    {
        public static bool IsNumeric(this Type type)
        {
            if (type == null)
                return false;

            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte => true,
                TypeCode.Decimal => true,
                TypeCode.Double => true,
                TypeCode.Int16 => true,
                TypeCode.Int32 => true,
                TypeCode.Int64 => true,
                TypeCode.SByte => true,
                TypeCode.Single => true,
                TypeCode.UInt16 => true,
                TypeCode.UInt32 => true,
                TypeCode.UInt64 => true,
                TypeCode.Object => 
                    type.IsGenericType && 
                    type.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    IsNumeric(Nullable.GetUnderlyingType(type)),
                _ => false
            };
        }
   }
}