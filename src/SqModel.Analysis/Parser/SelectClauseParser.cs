using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public static class SelectClauseParser
{
    public static SelectClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectClause Parse(TokenReader r)
    {
        var distinct = (r.TryReadToken("distinct") != null) ? true : false;
        if (r.TryReadToken("top") == null)
        {
            return new SelectClause(ReadItems(r).ToList()) { HasDistinctKeyword = distinct };
        }
        var top = ValueParser.Parse(r);
        return new SelectClause(ReadItems(r).ToList()) { HasDistinctKeyword = distinct, Top = top };
    }

    private static IEnumerable<SelectableItem> ReadItems(TokenReader r)
    {
        do
        {
            if (r.PeekToken().AreEqual(",")) r.ReadToken();
            yield return SelectableItemParser.Parse(r);
        }
        while (r.PeekToken().AreEqual(","));
    }
}