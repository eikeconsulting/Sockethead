using System;
using System.Collections.Generic;
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
        {
            if (expression.Body is MemberExpression me)
                return me;

            if (expression.Body is UnaryExpression ue)
            {
                if (ue.Operand is MemberExpression me2)
                    return me2;
            }

            return null;
        }

        public static string MemberName<T, V>(this Expression<Func<T, V>> expression)
            => expression.GetBody()?.Member.Name;

        public static T GetAttribute<T>(this ICustomAttributeProvider provider) where T : Attribute
            => provider.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;

        public static bool IsRequired<T, V>(this Expression<Func<T, V>> expression)
            => expression.GetBody()?.Member.GetAttribute<RequiredAttribute>() != null;

        /// <summary>
        /// Returns the name associated with the expression:
        /// 1. DisplayName attribute
        /// 2. Display.Name attribute
        /// 3. The underlying name of the field if no Display attribute adding spaces
        /// </summary>
        public static string FriendlyName<T, V>(this Expression<Func<T, V>> expression)
        {
            MemberExpression body = expression.GetBody();
            if (body == null)
                return null;

            var da = body.Member.GetAttribute<DisplayAttribute>();
            if (da != null && !string.IsNullOrEmpty(da.Name))
                return da.Name;

            var dna = body.Member.GetAttribute<DisplayNameAttribute>();
            if (dna != null && !string.IsNullOrEmpty(dna.DisplayName))
                return dna.DisplayName;

            return body.Member.Name?.PascalCaseAddSpaces();
        }

        public static Expression<Func<TModel, bool>> BuildWhere<TModel>(string propertyName, string inputText)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TModel), "x");
            Expression property = Expression.Property(parameter, propertyName);
            Expression target = Expression.Constant(inputText);
            Expression equalsMethod = Expression.Call(property, "Equals", null, target);
            Expression<Func<TModel, bool>> lambda =
               Expression.Lambda<Func<TModel, bool>>(equalsMethod, parameter);
            return lambda;
        }

        /*
        public static Expression<Func<TModel, T>> BuildExpression<TModel, T>(string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(typeof(TModel), "x");
            Expression property = Expression.Property(parameter, propertyName);
            Expression<Func<TModel, bool>> lambda =
               Expression.Lambda<Func<TModel, T>>();
            return lambda;
        }
        */

        public static Expression<Func<TModel, object>> GenerateGetterLambda<TModel>(PropertyInfo property)
        {
            // Define our instance parameter, which will be the input of the Func
            var objParameterExpr = Expression.Parameter(typeof(object), "instance");
            // 1. Cast the instance to the correct type
            var instanceExpr = Expression.TypeAs(objParameterExpr, property.DeclaringType);
            // 2. Call the getter and retrieve the value of the property
            var propertyExpr = Expression.Property(instanceExpr, property);
            // 3. Convert the property's value to object
            var propertyObjExpr = Expression.Convert(propertyExpr, typeof(object));
            // Create a lambda expression of the latest call & compile it
            return Expression.Lambda<Func<TModel, object>>(propertyObjExpr, objParameterExpr);
        }

        public static Dictionary<string, object> ModelToDictionary<T>(T model)
            => typeof(T)
                .GetProperties()
                .Select(pi => new { pi, lambda = GenerateGetterLambda<T>(pi) })
                .ToDictionary(
                    x => FriendlyName(x.lambda), 
                    x => x.lambda.Compile().Invoke(model));
    }
}
