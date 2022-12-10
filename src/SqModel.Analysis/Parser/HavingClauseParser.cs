using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public static class HavingClauseParser
{
    public static HavingClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static HavingClause Parse(TokenReader r)
    {
        var val = ValueParser.Parse(r);
        var having = new HavingClause(val);
        return having;
    }
}