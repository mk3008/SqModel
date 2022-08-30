using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Extension;

internal static class IEnumerableExtension
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
}
