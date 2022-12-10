using Cysharp.Text;

namespace SqModel.Core.Extensions;

internal static class IEnumerableExtension
{
    public static string ToString(this IEnumerable<string> source, string separator)
    {
        var sb = ZString.CreateStringBuilder();
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
            if (item == null || string.IsNullOrEmpty(item)) return sb.ToString();
            if (fn(item.ToString())) sb.Append(separator);
            prev = item.ToString();
            sb.Append(prev);
        }
        return sb.ToString();
    }
}
