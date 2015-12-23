using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Common
{
    public static class Util
    {
        public static void Each<T>(this IEnumerable<T> ls, Action<T> a)
        {
            if (ls != null)
                foreach (var l in ls)
                {
                    a(l);
                }
        }

        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> ls)
        {
            if (ls == null) yield break;
            foreach (var l in ls)
            {
                yield return l;
            }
        }
    }
}
