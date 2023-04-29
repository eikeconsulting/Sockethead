using System;
using System.Linq.Expressions;
using System.Reflection;
using Sockethead.Razor.Attributes;
using Sockethead.Razor.Helpers;

namespace Sockethead.Razor.Forms
{
    public partial class SimpleForm<T> where T : class
    {

        /// <summary>
        /// Build form from the model via Reflection
        /// </summary>
        public SimpleForm<T> BuildFormForModel()
        {
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (IgnoreFormGroup(property))
                    continue;
                
                ParameterExpression parameter = Expression.Parameter(typeof(T), "instance");
                MemberExpression propertyAccess = Expression.Property(parameter, property);

                switch (property.PropertyType.FullName)
                {
                    case "System.Boolean":
                        EditorFor(Expression.Lambda<Func<T, bool>>(propertyAccess, parameter));
                        break;

                    case "System.DateTime":
                        EditorFor(Expression.Lambda<Func<T, DateTime>>(propertyAccess, parameter));
                        break;

                    case "System.Decimal":
                        EditorFor(Expression.Lambda<Func<T, decimal>>(propertyAccess, parameter));
                        break;

                    case "System.Double":
                        EditorFor(Expression.Lambda<Func<T, double>>(propertyAccess, parameter));
                        break;

                    case "System.Single":
                        EditorFor(Expression.Lambda<Func<T, float>>(propertyAccess, parameter));
                        break;

                    case "System.Guid":
                        EditorFor(Expression.Lambda<Func<T, Guid>>(propertyAccess, parameter));
                        break;

                    case "System.Int16":
                        EditorFor(Expression.Lambda<Func<T, short>>(propertyAccess, parameter));
                        break;

                    case "System.Int32":
                        EditorFor(Expression.Lambda<Func<T, int>>(propertyAccess, parameter));
                        break;

                    case "System.Int64":
                        EditorFor(Expression.Lambda<Func<T, long>>(propertyAccess, parameter));
                        break;

                    case "System.String":
                        EditorFor(Expression.Lambda<Func<T, string>>(propertyAccess, parameter));
                        break;

                    case "System.TimeSpan":
                        EditorFor(Expression.Lambda<Func<T, TimeSpan>>(propertyAccess, parameter));
                        break;

                    case "System.UInt16":
                        EditorFor(Expression.Lambda<Func<T, ushort>>(propertyAccess, parameter));
                        break;

                    case "System.UInt32":
                        EditorFor(Expression.Lambda<Func<T, uint>>(propertyAccess, parameter));
                        break;

                    case "System.UInt64":
                        EditorFor(Expression.Lambda<Func<T, ulong>>(propertyAccess, parameter));
                        break;
                    
                    // ToDo: Add support for Nullable and other types

                    default:
                        // We are not supporting other types at this moment
                        break;
                }
            }
            
            return this;
        }

        private bool IgnoreFormGroup(PropertyInfo property)
        {
            Expression<Func<T, object>> expression = ExpressionHelpers.BuildGetterLambda<T>(property);
            return expression.GetAttribute<FormBuilderIgnore, T, object>() != null;
        }
    }
}