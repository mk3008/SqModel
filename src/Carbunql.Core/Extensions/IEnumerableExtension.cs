using Carbunql.Core.Values;
using Cysharp.Text;
using System.Text;

namespace Carbunql.Core.Extensions;

public static class IEnumerableExtension
{
    public static string ToString(this IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> source, string separator)
    {
        var sb = ZString.CreateStringBuilder();
        (Type sender, string text, BlockType block, bool isReserved)? prev = null;

        var isAppendSplitter = ((Type sender, string text, BlockType block, bool isReserved) current) =>
        {
            if (prev == null) return false;
            //if (current.block == BlockType.Split) return true;

            if (prev.Value.text == "(") return false;
            if (current.text == ")") return false;
            if (current.text == ",") return false;
            if (prev.Value.text == ".") return false;
            if (current.text == ".") return false;
            if (current.text == "(" && prev.Value.sender.Equals(typeof(FunctionValue))) return false;

            return true;
        };

        foreach (var item in source)
        {
            if (string.IsNullOrEmpty(item.text)) continue;
            if (isAppendSplitter(item)) sb.Append(separator);
            sb.Append(item.text);
            prev = item;
        }
        return sb.ToString();
    }
}
