﻿using SqModel.Analysis.Extensions;
using SqModel.Core;
using SqModel.Core.Clauses;
using SqModel.Core.Values;

namespace SqModel.Analysis.Parser;

public static class ValuesClauseParser
{
    public static ValuesClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static ValuesClause Parse(TokenReader r)
    {
        var fn = () =>
        {
            if (!r.PeekRawToken().AreEqual(",")) return false;
            r.ReadToken(",");
            r.ReadToken("(");
            return true;
        };

        r.TryReadToken("values");
        r.ReadToken("(");

        var lst = new List<ValueCollection>();
        do
        {
            var (_, inner) = r.ReadUntilCloseBracket();
            lst.Add(ValueCollectionParser.Parse(inner));
        } while (fn());

        return new ValuesClause(lst);
    }
}