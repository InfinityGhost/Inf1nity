using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Tools
{
    public static class ReadWriteHelper
    {
        public static string Fetch(this IEnumerable<string> content, string prefix, string splitter = ":")
        {
            return content.Where(e => e.Contains(prefix)).First().Replace(prefix + splitter, string.Empty) ?? throw new InvalidOperationException();
        }
    }
}
