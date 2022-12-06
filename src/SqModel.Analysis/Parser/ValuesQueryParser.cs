using SqModel.Analysis.Builder;
using SqModel.Analysis.Extensions;
using SqModel.Core.Tables;
using SqModel.Core.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Parser;

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