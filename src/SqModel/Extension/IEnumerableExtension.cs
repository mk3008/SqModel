using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Extension;

internal static class IEnumerableExtension
{
    public static bool Contains(this IEnumerable<char> source, string? value)
    {
        if (value == null || value.IsEmpty()) return false;
        if (value.Length != 1) return false;
        return source.Contains(value.First());
    }

    public static string ToString<T>(this IEnumerable<T> source, string separator)
    {
        var sb = new StringBuilder();
        var prev = string.Empty;

        var fn = (string? current) =>
        {
            if (prev == string.Empty) return false;
            if (prev == "(") return false;
            if (current == ")") return false;
            if (prev == ".") return false;
            if (current == ".") return false;

            return true;
        };

        foreach (var item in source)
        {
            if (item == null || item.ToString().IsEmpty()) return sb.ToString();
            if (fn(item.ToString())) sb.Append(separator);
            prev = item.ToString();
            sb.Append(prev);
        }
        return sb.ToString();
    }
}
