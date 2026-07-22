using System;
using System.Collections.Generic;

namespace Bas.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool TryGetValue<T>(this IEnumerable<T> enumerable, Func<T, bool> func, out T value)
        {
            foreach (T item in enumerable)
            {
                if (func(item))
                {
                    value = item;
                    return true;
                }
            }

            value = default;
            return false;
        }

        public static bool TryRemove<T>(this List<T> list, Func<T, bool> func)
        {
            for (int i = 0; i < list.Count; i++)
            {
                T item = list[i];
                if (func(item))
                {
                    list.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
        }
    }
}