using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace g2b.Common
{
    using System.Globalization;
    using System.IO;

    public class LastSync
    {
        private const string Fp = ".LastSync";

        public static string Get()
        {
            var ret = File.Exists(Fp) ? File.ReadAllText(Fp) : null;
            Console.Out.WriteLine("{0} Get: {1}", Fp, ret);
            return ret;
        }

        public static void Set(string t)
        {
            File.WriteAllText(Fp, t);
            Console.Out.WriteLine("{0} Set: {1}", Fp, t);
        }

        public const string DateTimeFormat = @"yyyyMMdd HHmmss zzz";
        public const string DateTimeFormatShort = @"yyyyMMdd HHmmss";

        public static string Format(DateTime t)
        {
            return t.ToString(DateTimeFormatShort) + " UTC";
        }

        //public static DateTime? Get()
        //{
        //    DateTime? ret = null;
        //    if (File.Exists(Fp))
        //        ret = DateTime.ParseExact(File.ReadAllText(Fp), DateTimeFormat, CultureInfo.InvariantCulture);
        //    Console.Out.WriteLine(Fp + " GET :" + ret);
        //    return ret;
        //}

        //public static void Set(DateTime t)
        //{
        //    File.WriteAllText(Fp, t.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
        //    Console.Out.WriteLine("update " + Fp);
        //}
        
        //public static void ParseAndSet(string t)
        //{
        //    t = t.Substring(0, t.Length - 4);
        //    var d= DateTime.ParseExact(t, DateTimeFormatShort, CultureInfo.InvariantCulture);
        //    d.
        //    Set(d);
        //}
    }
}
