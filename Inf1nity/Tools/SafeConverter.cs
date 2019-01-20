using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity.Tools
{
    public static class SafeConverter
    {
        public static string String(this object obj, string fallback = "")
        {
            try
            {
                return obj.ToString();
            }
            catch
            {
                return fallback;
            }
        }

        public static int Int32(this object obj, int fallback = 0)
        {
            try
            {
                return Convert.ToInt32(obj);
            }
            catch
            {
                return fallback;
            }
        }

        public static bool Boolean(this object obj, bool fallback = false)
        {
            try
            {
                return Convert.ToBoolean(obj);
            }
            catch
            {
                return fallback;
            }
        }
    }
}
