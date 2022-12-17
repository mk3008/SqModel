﻿using Carbunql.Core.Extensions;
using Carbunql.Core;
using Carbunql.Core.Clauses;
using Carbunql.Core.Values;

namespace Carbunql.Analysis.Parser;

public static class CommonTableParser
{
    public static CommonTable Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static CommonTable Parse(TokenReader r)
    {
        var breaktokens = TokenReader.BreakTokens;

        var alias = r.ReadToken();
        ValueCollection? colAliases = null;
        if (r.PeekRawToken().AreEqual("("))
        {
            r.ReadToken("(");
            var (_, names) = r.ReadUntilCloseBracket();
            colAliases = ValueCollectionParser.Parse(names);
        }

        r.ReadToken("as");

        var material = MaterializedType.Undefined;
        if (r.PeekRawToken().AreEqual("materialized"))
        {
            r.ReadToken("materialized");
            material = MaterializedType.Materialized;
        }
        else if (r.PeekRawToken().AreEqual("not"))
        {
            r.ReadToken("not");
            material = MaterializedType.NotMaterialized;
        }

        r.ReadToken("(");
        var (first, inner) = r.ReadUntilCloseBracket();
        if (first.AreContains(new[] { "select", "values" }))
        {
            var t = TableParser.Parse("(" + inner + ")");
            if (colAliases != null)
            {
                return new CommonTable(t, alias, colAliases) { Materialized = material };
            }
            else
            {
                return new CommonTable(t, alias) { Materialized = material };
            }
        }
        throw new NotSupportedException();
    }
}