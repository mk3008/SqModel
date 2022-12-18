using Carbunql.Core.Values;
using Cysharp.Text;
using System.Text;

namespace Carbunql.Core.Extensions;

public static class IEnumerableExtension
{
    public static string ToString(this IEnumerable<Token> source, string separator)
    {
        var sb = ZString.CreateStringBuilder();
        Token? prev = null;

        var isAppendSplitter = (Token current) =>
        {
            if (prev == null) return false;

            if (prev!.Text == "(") return false;
            if (current.Text == ")") return false;
            if (current.Text == ",") return false;
            if (prev!.Text == ".") return false;
            if (current.Text == ".") return false;
            if (current.Text == "(" && prev!.IsReserved) return false;
            return true;
        };

        foreach (var item in source)
        {
            if (string.IsNullOrEmpty(item.Text)) continue;
            if (isAppendSplitter(item)) sb.Append(separator);
            sb.Append(item.Text);
            prev = item;
        }
        return sb.ToString();
    }
}
