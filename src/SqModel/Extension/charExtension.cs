using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Serialization;

namespace SqModel.Extension;

internal static class charExtension
{
    public static bool IsSpace(this char source)
        => SqlParser.SpaceTokens.Where(x => x == source).Any();

    public static bool IsSpace(this char? source)
        => source == null ? false : source.Value.IsSpace();

    public static bool IsSymbol(this char source)
        => SqlParser.SymbolTokens.Where(x => x == source).Any();

    public static bool IsSymbol(this char? source)
        => source == null ? false : source.Value.IsSymbol();
}
