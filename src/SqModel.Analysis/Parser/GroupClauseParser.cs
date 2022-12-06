﻿using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public class GroupClauseParser
{
    public static GroupClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static GroupClause Parse(TokenReader r)
    {
        var vals = ValueCollectionParser.Parse(r);
        var group = new GroupClause(vals);
        return group;
    }
}
