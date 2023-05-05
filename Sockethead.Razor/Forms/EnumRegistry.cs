using System;
using System.Linq.Expressions;

namespace Sockethead.Razor.Forms
{
    public abstract class EnumRegistry<TModel>  where TModel : class
    {
        public abstract bool ResolveEnum(SimpleForm<TModel> form, LambdaExpression expression);
    }
    
    public class EnumRegistry<TModel, TEnum1> : EnumRegistry<TModel> where TModel : class
    {
        public override bool ResolveEnum(SimpleForm<TModel> form, LambdaExpression expression)
        {
            if (expression is not Expression<Func<TModel, TEnum1>> ex)
                return false;

            form.AddEnumRowFor(ex);
            
            return false;
        }
    }
    
    public class EnumRegistry<TModel, TEnum1, TEnum2> : EnumRegistry<TModel, TEnum1> where TModel : class
    {
        public override bool ResolveEnum(SimpleForm<TModel> form, LambdaExpression expression)
        {
            if (base.ResolveEnum(form, expression))
                return true;
            
            if (expression is not Expression<Func<TModel, TEnum2>> ex)
                return false;

            form.AddEnumRowFor(ex);
            
            return false;
        }
    }

    public class EnumRegistry<TModel, TEnum1, TEnum2, TEnum3> : EnumRegistry<TModel, TEnum1, TEnum2> where TModel : class
    {
        public override bool ResolveEnum(SimpleForm<TModel> form, LambdaExpression expression)
        {
            if (base.ResolveEnum(form, expression))
                return true;
            
            if (expression is not Expression<Func<TModel, TEnum3>> ex)
                return false;

            form.AddEnumRowFor(ex);
            
            return false;
        }
    }
}