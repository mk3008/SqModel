using SqModel.Analysis.Extensions;
using SqModel.Analysis.Parser;
using SqModel.Core;
using SqModel.Core.Clauses;

namespace SqModel.Analysis;

public static class SelectQueryParser
{
    public static SelectQuery Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectQuery Parse(TokenReader r)
    {
        var token = r.ReadToken();
        if (!token.AreEqual("select")) throw new NotSupportedException("Initial token must be 'select'.");

        var sq = new SelectQuery();
        sq.SelectClause = SelectClauseParser.Parse(r);
        sq.FromClause = ParseFromOrDefault(r);
        sq.WhereClause = ParseWhereOrDefault(r);
        sq.GroupClause = ParseGroupOrDefault(r);
        sq.HavingClause = ParseHavingOrDefault(r);
        sq.OrderClause = ParseOrderOrDefault(r);

        return sq;
    }

    private static FromClause? ParseFromOrDefault(TokenReader r)
    {
        if (!r.PeekToken().AreEqual("from")) return null;
        r.ReadToken();
        return FromClauseParser.Parse(r);
    }

    private static WhereClause? ParseWhereOrDefault(TokenReader r)
    {
        if (!r.PeekToken().AreEqual("where")) return null;
        r.ReadToken();
        return WhereClauseParser.Parse(r);
    }

    private static GroupClause? ParseGroupOrDefault(TokenReader r)
    {
        if (!r.PeekToken().AreEqual("group by")) return null;
        r.ReadToken();
        return GroupClauseParser.Parse(r);
    }

    private static HavingClause? ParseHavingOrDefault(TokenReader r)
    {
        if (!r.PeekToken().AreEqual("having")) return null;
        r.ReadToken();
        return HavingClauseParser.Parse(r);
    }

    private static OrderClause? ParseOrderOrDefault(TokenReader r)
    {
        if (!r.PeekToken().AreEqual("order by")) return null;
        r.ReadToken();
        return OrderClauseParser.Parse(r);
    }
}