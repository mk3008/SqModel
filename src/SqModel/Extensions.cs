using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

internal static class Extensions
{
    public static string ToString<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }

    public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> source, Dictionary<T1, T2> dic) where T1 : notnull
    {
        dic.Where(x => !source.ContainsKey(x.Key)).ToList().ForEach(x => source.Add(x.Key, x.Value));
        return source;
    }
}
