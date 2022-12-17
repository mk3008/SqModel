﻿using Carbunql.Core.Extensions;
using Carbunql.Core.Clauses;
using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public static class ValueCollectionParser
{
    public static ValueCollection Parse(string text)
    {
        using var r = new TokenReader(text);
        return new ValueCollection(ReadValues(r).ToList());
    }

    public static ValueCollection Parse(TokenReader r)
    {
        return new ValueCollection(ReadValues(r).ToList());
    }

    private static IEnumerable<ValueBase> ReadValues(TokenReader r)
    {
        do
        {
            if (r.PeekRawToken().AreEqual(",")) r.ReadToken();
            yield return ValueParser.Parse(r);
        }
        while (r.PeekRawToken().AreEqual(","));
    }
}