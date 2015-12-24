using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Common
{
    using System.IO;

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

        public static IDictionary<TKey, TValue> ToDictionaryDuplicateKeyOverride<T, TKey, TValue>(this IEnumerable<T> x,
            Func<T, TKey> fk, Func<T, TValue> fv)
        {
            var ret = new Dictionary<TKey, TValue>();
            foreach (var o in x)
            {
                ret[fk(o)] = fv(o);
            }
            return ret;
        }

        public static TValue TryGet<TKey, TValue>(this IDictionary<TKey, TValue> x, TKey key) where TValue: class
        {
            TValue ret;
            return x.TryGetValue(key, out ret) ? ret : null;
        }

        public static void Write<TKey, TValue>(this IDictionary<TKey, TValue> x, string f)
        {
            if (x == null) return;
            using (var o = new StreamWriter(f))
            {
                foreach (var kv in x)
                {
                    o.WriteLine("{0}\t{1}", kv.Key, kv.Value);
                }
            }
        }

        public static IDictionary<long, long> ReadMap(string f)
        {
            return
                File.ReadLines(f)
                    .Select(l => l.Split('\t'))
                    .Where(a => a.Length == 2)
                    .ToDictionaryDuplicateKeyOverride(a => long.Parse(a[0]), a => long.Parse(a[1]));
        } 
    }
}
