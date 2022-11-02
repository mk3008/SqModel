using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SqModel.Analysis;
using SqModel.Extension;

namespace SqModel.Extension;

internal static class charExtension
{
    public static bool Contains(this char[]? source, char? item)
    {
        if (source == null) return false;
        if (item == null) return false;
        return source.Where(x => x == item).Any();
    }

    public static bool IsSpace(this char source)
        => SqlParser.SpaceTokens.Where(x => x == source).Any();

    public static bool IsSpace(this char? source)
        => source == null ? false : source.Value.IsSpace();

    public static bool IsSymbol(this char source)
        => SqlParser.SymbolTokens.Where(x => x == source).Any();

    public static bool IsSymbol(this char? source)
        => source == null ? false : source.Value.IsSymbol();

    public static bool IsNumeric(this char? source)
        => source == null ? false : "0123456789".Any(source.Value);
}
