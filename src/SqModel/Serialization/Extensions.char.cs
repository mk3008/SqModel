using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

internal static partial class Extensions
{
    public static bool IsSpace(this char source)
        => Parser.SpaceTokens.Where(x => x == source).Any();

    public static bool IsSpace(this char? source)
        => (source == null) ? false : source.Value.IsSpace();

    public static bool IsSymbol(this char source)
        => Parser.SymbolTokens.Where(x => x == source).Any();

    public static bool IsSymbol(this char? source)
        => (source == null) ? false : source.Value.IsSymbol();
}
