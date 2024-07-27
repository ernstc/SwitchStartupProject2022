using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwitchStartupProject
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Generic foreach method for enumerables
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> that, Action<T> action)
        {
            foreach (T item in that)
            {
                action(item);
            }
        }
    }
}
