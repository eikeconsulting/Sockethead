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

            if (expression.Body is UnaryExpression ue && 
                ue.Operand is MemberExpression me2)
                return me2;

            return null;
        }

        public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider provider) where TAttribute : Attribute
            => provider
                .GetCustomAttributes(typeof(TAttribute), true)
                .FirstOrDefault() as TAttribute;

        public static TAttribute GetAttribute<TAttribute, T, V>(this Expression<Func<T, V>> expression) where TAttribute : Attribute
            => expression
                .GetBody()
                ?.Member
                .GetAttribute<TAttribute>();

        /// <summary>
        /// Returns the name associated with the expression:
        /// 1. Display.Name attribute
        /// 2. DisplayName attribute
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

        /// <summary>
        /// Build a lambda expression for a Where clause (i.e. returns a bool)
        /// for a property (given by name) that Equals a specific string
        /// The final Lambda looks like: 
        /// x => x.MyProp.Equals("mysearch")
        /// </summary>
        /// <typeparam name="TModel">Model type</typeparam>
        /// <param name="propertyName">Name of the property to compare</param>
        /// <param name="inputText">String to match (Equals)</param>
        /// <returns>Lambda Expression</returns>
        public static Expression<Func<TModel, bool>> BuildWhereEqualsLambda<TModel>(string propertyName, string inputText)
        {
            // Create the lambda input parameter (i.e. "x => ..." part)
            ParameterExpression parameter = Expression.Parameter(typeof(TModel), "x");

            // Create the property part (i.e. "x.MyProp" part)
            MemberExpression property = Expression.Property(parameter, propertyName);

            // Create the target for the comparision (i.e. "mysearch")
            ConstantExpression target = Expression.Constant(inputText);

            // Create an Equals clause (i.e. "x.MyProp.Equals("mysearch")")
            MethodCallExpression equalsMethod = Expression.Call(property, "Equals", null, target);
            
            // Put it all together (i.e. "x => x.MyProp.Equals("mysearch")")
            return Expression.Lambda<Func<TModel, bool>>(equalsMethod, parameter);
        }

        /// <summary>
        /// Build a lambda expression for "get" from a Property
        /// The final Lambda looks like: 
        /// x => x.MyProp
        /// </summary>
        /// <typeparam name="TModel">Model Type</typeparam>
        /// <param name="property">PropertyInfo for the Property</param>
        /// <returns>Lambda Expression</returns>
        public static Expression<Func<TModel, object>> BuildGetterLambda<TModel>(PropertyInfo property)
        {
            // Define our instance parameter, which will be the input of the Func
            ParameterExpression objParameterExpr = Expression.Parameter(typeof(object), "instance");
            
            // Cast the instance to the correct type
            UnaryExpression instanceExpr = Expression.TypeAs(objParameterExpr, property.DeclaringType);
            
            // Call the getter and retrieve the value of the property
            MemberExpression propertyExpr = Expression.Property(instanceExpr, property);
            
            // Convert the property's value to object
            UnaryExpression propertyObjExpr = Expression.Convert(propertyExpr, typeof(object));
            
            // Create a lambda expression of the latest call & compile it
            return Expression.Lambda<Func<TModel, object>>(propertyObjExpr, objParameterExpr);
        }

        /// <summary>
        /// Extract all Properties of the object (model) and 
        /// create a Dictionary of Name/Value
        /// </summary>
        /// <typeparam name="T">Type of the object (must be known at compile time)</typeparam>
        /// <param name="model">object to convert</param>
        public static Dictionary<string, object> ModelToDictionary<T>(T model)
            => typeof(T)
                .GetProperties()
                .Select(pi => new { pi, lambda = BuildGetterLambda<T>(pi) })
                .Where(x =>
                {
                    DisplayAttribute display = x.lambda.GetAttribute<DisplayAttribute, T, object>();

                    // Skip if DisplayAttribute.AutoGenerateField is turned off
                    return
                        display == null ||
                        !display.GetAutoGenerateField().HasValue ||
                        display.GetAutoGenerateField().Value;
                })
                .ToDictionary(
                    x => FriendlyName(x.lambda), 
                    x => x.lambda.Compile().Invoke(model));
    }
}
