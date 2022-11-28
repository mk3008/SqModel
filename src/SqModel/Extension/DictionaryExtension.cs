using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Extension;

internal static class DictionaryExtension
{
    public static void ForEach<T1, T2>(this Dictionary<T1, T2> source, Action<KeyValuePair<T1, T2>> action) where T1 : notnull
    {
        foreach (var x in source) action(x);
    }

    public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> source, Dictionary<T1, T2>? dic) where T1 : notnull
    {
        if (dic == null) return source;
        dic.ForEach(x => source[x.Key] = x.Value);
        return source;
    }

    public static Dictionary<T1, T2>? MergeOrDefault<T1, T2>(this Dictionary<T1, T2>? source, Dictionary<T1, T2>? dic) where T1 : notnull
    {
        if (source == null) return dic;
        if (dic == null) return source;
        dic.ForEach(x => source[x.Key] = x.Value);
        return source;
    }


}
