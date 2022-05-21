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
        return source.Concat(dic).Where(x => !dic.ContainsKey(x.Key)).ToDictionary(x => x.Key, x => x.Value);
    }

    public static Dictionary<T1, T2> Merge<T1, T2>(this List<Dictionary<T1, T2>> source) where T1 : notnull
    {
        var dic = new Dictionary<T1, T2>();
        source.ForEach(x => dic.Merge(x));
        return dic;
    }
}
