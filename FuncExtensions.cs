using System;

namespace FastHashtable
{
    internal static class FuncExtensions
    {
        // hack of avoid closure allocation(() => value).
        public static Func<TIgnore, T> AsFunc<TIgnore, T>(this T value)
        {
            return new Func<TIgnore, T>(value.ReturnBox<TIgnore, T>);
        }

        static T ReturnBox<TIgnore, T>(this object value, TIgnore _)
        {
            return (T)value;
        }
    }
}