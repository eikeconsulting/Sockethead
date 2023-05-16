using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Sockethead.Razor.Helpers;

namespace Sockethead.Razor.Forms
{
    public class SimpleFormMagic<T> where T : class
    {
        private SimpleForm<T> Form { get; }
        private EnumRegistry<T> EnumRegistry { get; }

        public SimpleFormMagic(SimpleForm<T> form, EnumRegistry<T> enumRegistry)
        {
            Form = form;
            EnumRegistry = enumRegistry;
        }

        private static PropertyInfo[] GetPropertiesOrderedByBaseFirst(Type type) =>
            type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(p => p.DeclaringType == type ? 1 : 0)
                .ToArray();
        
        public void AddRowsForModel()
        {
            foreach (PropertyInfo pi in GetPropertiesOrderedByBaseFirst(typeof(T)))
                AddRow(ExpressionHelpers.GenerateLambdaExpressionForProperty<T>(pi));
        }

        private void AddRow(LambdaExpression expression)
        {
            if (EnumRegistry != null && EnumRegistry.ResolveEnum(Form, expression))
                return;

            switch (expression)
            {
                case Expression<Func<T, string>> ex: _AddRow(ex); break;

                case Expression<Func<T, short>> ex: _AddRow(ex); break;
                case Expression<Func<T, int>> ex: _AddRow(ex); break;
                case Expression<Func<T, float>> ex: _AddRow(ex); break;
                case Expression<Func<T, double>> ex: _AddRow(ex); break;
                case Expression<Func<T, decimal>> ex: _AddRow(ex); break;
                
                case Expression<Func<T, bool>> ex: _AddRow(ex); break;
                case Expression<Func<T, Guid>> ex: _AddRow(ex); break;
                case Expression<Func<T, DateTime>> ex: _AddRow(ex); break;
            }
        }
    
        private void _AddRow<TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression.GetAttribute<ScaffoldColumnAttribute, T, TResult>() is {Scaffold: false})
                return;
            Form.AddRowFor(expression);
        }             
    }
}
