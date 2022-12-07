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
            r.ReadToken();
            if (!r.PeekToken().AreEqual("(")) throw new SyntaxException("near ,");
            r.ReadToken();
            return true;
        };

        var item = r.ReadToken();
        if (!item.AreEqual("values")) throw new SyntaxException("");

        item = r.ReadToken();
        var lst = new List<ValueCollection>();
        do
        {
            if (item != "(") throw new SyntaxException("near values");

            var (_, inner) = r.ReadUntilCloseBracket();
            lst.Add(ValueCollectionParser.Parse(inner));

        } while (fn());

        return new ValuesQuery(lst);
    }
}