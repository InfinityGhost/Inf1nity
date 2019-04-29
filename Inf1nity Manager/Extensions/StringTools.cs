using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Tools
{
    static class StringTools
    {
        public static IEnumerable<string> SplitLines(this string lines)
        {
            return lines.Split(new string[] { Environment.NewLine }, 0);
        }

        public static string Combine(this IEnumerable<string> lines)
        {
            string ret = string.Empty;
            foreach(string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(ret))
                    ret += Environment.NewLine;
                ret += line;
            }
            return ret;
        }
    }
}
