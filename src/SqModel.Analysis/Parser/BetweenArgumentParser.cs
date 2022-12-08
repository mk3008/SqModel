using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Parser;

public static class BetweenArgumentParser
{
    public static BetweenArgument Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static BetweenArgument Parse(TokenReader r)
    {
        var start = ValueParser.ParseCore(r);
        r.ReadToken("and");
        var end = ValueParser.ParseCore(r);
        return new BetweenArgument(start, end);
    }
}
