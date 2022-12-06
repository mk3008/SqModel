using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public static class SelectClauseParser
{
    public static SelectClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return new SelectClause(ReadItems(r).ToList());
    }

    public static SelectClause Parse(TokenReader r)
    {
        return new SelectClause(ReadItems(r).ToList());
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
