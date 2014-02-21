using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liara.Extensions
{
    public static class Misc
    {
        public static TValue TryGetValue<TKey, TValue>(
            Dictionary<TKey, TValue> dict, TKey value, out bool found)
        {
            TValue result;
            found = dict.TryGetValue(value, out result);
            return result;
        }
    }
}
