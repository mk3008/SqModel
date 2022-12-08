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
        r.ReadToken("select");

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
        if (r.TryReadToken("from") == null) return null;
        return FromClauseParser.Parse(r);
    }

    private static WhereClause? ParseWhereOrDefault(TokenReader r)
    {
        if (r.TryReadToken("where") == null) return null;
        return WhereClauseParser.Parse(r);
    }

    private static GroupClause? ParseGroupOrDefault(TokenReader r)
    {
        if (r.TryReadToken("group by") == null) return null;
        return GroupClauseParser.Parse(r);
    }

    private static HavingClause? ParseHavingOrDefault(TokenReader r)
    {
        if (r.TryReadToken("having") == null) return null;
        return HavingClauseParser.Parse(r);
    }

    private static OrderClause? ParseOrderOrDefault(TokenReader r)
    {
        if (r.TryReadToken("order by") == null) return null;
        return OrderClauseParser.Parse(r);
    }
}