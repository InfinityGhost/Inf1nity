using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager.Controls.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange(this Dictionary<object, object> dictionary, Dictionary<object, object> items)
        {
            foreach(var item in items)
                dictionary.Add(item.Key, item.Value);
        }
    }
}
