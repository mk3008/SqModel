using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

internal static class stringExtension
{
    public static string InsertIndent(this string source, string separator = "\r\n", int spaceCount = 4)
    {
        if (source.IsEmpty()) return source;

        var indent = spaceCount.ToSpaceString();

        return $"{indent}{source.Replace(separator, $"{separator}{indent}")}";
    }

    public static bool IsEmpty(this string? source)
        => string.IsNullOrEmpty(source);

    public static bool IsNotEmpty(this string? source)
        => !string.IsNullOrEmpty(source);

    public static Dictionary<string, string> ToDictionary(this List<string> source)
    {
        var dic = new Dictionary<string, string>();
        source.ForEach(x => dic.Add(x, x));
        return dic;
    }
}
