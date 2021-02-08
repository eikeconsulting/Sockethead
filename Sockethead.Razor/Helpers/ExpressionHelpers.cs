using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Sockethead.Razor.Helpers
{
    public static class ExpressionHelpers
    {
        public static MemberExpression GetBody<T, V>(this Expression<Func<T, V>> expression)
            => expression.Body is MemberExpression body
                ? body
                : ((UnaryExpression)expression.Body).Operand as MemberExpression;

        public static string MemberName<T, V>(this Expression<Func<T, V>> expression)
            => expression.GetBody().Member.Name;

        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
            => provider.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;

        public static bool IsRequired<T, V>(this Expression<Func<T, V>> expression)
            => expression.GetBody().Member.GetAttribute<RequiredAttribute>() != null;

        /// <summary>
        /// Returns the name associated with the expression:
        /// 1. DisplayName attribute
        /// 2. Display.Name attribute
        /// 3. The underlying name of the field if no Display attribute
        /// </summary>
        public static string FriendlyName<T, V>(this Expression<Func<T, V>> expression)
        {
            MemberExpression body = expression.GetBody();

            var dna = body.Member.GetAttribute<DisplayNameAttribute>();
            if (dna != null && !string.IsNullOrEmpty(dna.DisplayName))
                return dna.DisplayName;

            var da = body.Member.GetAttribute<DisplayAttribute>();
            if (da != null && !string.IsNullOrEmpty(da.Name))
                return da.Name;

            return body.Member.Name;
        }
    }
}
