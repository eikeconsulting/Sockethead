using System;
using System.Linq.Expressions;
using System.Reflection;
using Sockethead.Razor.Attributes;
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

        public void AddRowsForModel()
        {
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                LambdaExpression expression = ExpressionHelpers.GenerateLambdaExpressionForProperty<T>(propertyInfo); 
                AddRow(expression);
            }

            void AddRow(LambdaExpression expression)
            {
                if (EnumRegistry != null && EnumRegistry.ResolveEnum(Form, expression))
                    return;

                switch (expression)
                {
                    case Expression<Func<T, string>> ex1: Handle(ex1); break;
                    case Expression<Func<T, int>> ex2: Handle(ex2); break;
                    case Expression<Func<T, float>> ex3: Handle(ex3); break;
                    case Expression<Func<T, double>> ex4: Handle(ex4); break;
                    case Expression<Func<T, bool>> ex5: Handle(ex5); break;
                    
                    // TODO handle more types
                }
                
            }
            
            void Handle<TProperty>(Expression<Func<T, TProperty>> expression)
            {
                FormBuilderIgnore ignore = expression.GetAttribute<FormBuilderIgnore, T, TProperty>();
            
                // Skip if the property has FormBuilderIgnore attribute
                if (ignore != null)
                    return;

                switch (typeof(TProperty))
                {
                    case var type when type == typeof(bool):
                        Form.AddCheckBoxRowFor(expression as Expression<Func<T, bool>>);
                        break;
                    
                    default:
                        Form.AddRowFor(expression);
                        break;
                }
            }             
        }
    }
}