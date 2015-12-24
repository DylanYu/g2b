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

        public static DateTime? Get()
        {
            DateTime? ret = null;
            if (File.Exists(Fp))
                ret= DateTime.ParseExact(File.ReadAllText(Fp), "s", CultureInfo.InvariantCulture);
            Console.Out.WriteLine(Fp+" GET :"+ret);
            return ret;
        }

        public static void Set(DateTime t)
        {
            File.WriteAllText(Fp, t.ToString("s", CultureInfo.InvariantCulture));
            Console.Out.WriteLine("update " + Fp);
        }

    }
}
