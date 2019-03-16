using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inf1nity_Manager.Extensions
{
    public static class DictionaryExtensions
    {
        public static void AddRange<T1, T2> (this Dictionary<T1, T2> dictionary, List<T1> keys, List<T2> values)
        {
            if (keys.Count() != values.Count())
                throw new ArgumentException("Size of lists must be equal.");

            for (int i = 0; i < keys.Count(); i++)
                dictionary.Add(keys[i], values[i]);
        }

        public static void Add<T1, T2> (this Dictionary<T1, T2> dictionary, KeyValuePair<T1, T2> keyPair)
        {
            dictionary.Add(keyPair.Key, keyPair.Value);
        }
    }
}
