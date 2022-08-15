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
        var sb = new StringBuilder();
        var prev = string.Empty;
        foreach (var item in source)
        {
            if (item == null || item.ToString().IsEmpty()) throw new Exception();
            if (prev != string.Empty && prev != "(" && item.ToString() != ")") sb.Append(separator);
            prev = item.ToString();
            sb.Append(prev);
        }
        return sb.ToString();
    }

    public static void ForEach<T1, T2>(this Dictionary<T1, T2> source, Action<KeyValuePair<T1, T2>> action) where T1 : notnull
    {
        foreach (var x in source) action(x);
    }

    public static Dictionary<T1, T2> Merge<T1, T2>(this Dictionary<T1, T2> source, Dictionary<T1, T2> dic) where T1 : notnull
    {
        dic.ForEach(x => source[x.Key] = x.Value);
        return source;
    }

    public static Dictionary<string, string> ToDictionary(this List<string> source)
    {
        var dic = new Dictionary<string, string>();
        source.ForEach(x => dic.Add(x, x));
        return dic;
    }

    public static Query ToQuery(this List<Query> source, string separator)
    {
        var text = source.Select(x => x.CommandText).ToList().ToString(separator);
        var prm = new Dictionary<string, object>();
        source.ForEach(x => prm.Merge(x.Parameters));

        return new Query() { CommandText = text, Parameters = prm };
    }

    public static Query Merge(this Query source, Query query, string separator)
    {
        var text = source.CommandText;
        if (source.CommandText.IsEmpty()) text = query.CommandText;
        else if (query.CommandText.IsNotEmpty()) text += $"{separator}{query.CommandText}";

        var prm = new Dictionary<string, object>();
        prm.Merge(source.Parameters);
        prm.Merge(query.Parameters);

        return new Query() { CommandText = text, Parameters = prm };
    }

    public static void ForEach(this int source, Action<int> action)
    {
        for (int i = 0; i < source; i++) action(i);
    }

    public static string Space(this int source)
    {
        var space = string.Empty;
        source.ForEach(x => space += " ");
        return space;
    }

    public static string InsertIndent(this string source, string separator = "\r\n", int spaceCount = 4)
    {
        if (source.IsEmpty()) return source;

        var indent = spaceCount.Space();

        return $"{indent}{source.Replace(separator, $"{separator}{indent}")}";
    }

    public static bool IsEmpty(this string? source)
        => string.IsNullOrEmpty(source);
    public static bool IsNotEmpty(this string? source)
    => !string.IsNullOrEmpty(source);

    public static bool IsEmpty(this Query? source)
    => string.IsNullOrEmpty(source?.CommandText);
    public static bool IsNotEmpty(this Query? source)
    => !string.IsNullOrEmpty(source?.CommandText);
}
