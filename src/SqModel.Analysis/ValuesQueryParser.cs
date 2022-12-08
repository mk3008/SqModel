using SqModel.Analysis.Extensions;
using SqModel.Analysis.Parser;
using SqModel.Core;
using SqModel.Core.Values;

namespace SqModel.Analysis;

public static class ValuesQueryParser
{
    public static ValuesQuery Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ValuesQuery Parse(TokenReader r)
    {
        var fn = () =>
        {
            if (!r.PeekToken().AreEqual(",")) return false;
            r.ReadToken(",");
            r.ReadToken("(");
            return true;
        };

        r.ReadToken("values");
        r.ReadToken("(");

        var lst = new List<ValueCollection>();
        do
        {
            var (_, inner) = r.ReadUntilCloseBracket();
            lst.Add(ValueCollectionParser.Parse(inner));
        } while (fn());

        return new ValuesQuery(lst);
    }
}