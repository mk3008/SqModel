using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public static class SortableItemParser
{
    public static SortableItem Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SortableItem Parse(TokenReader r)
    {
        var breaktokens = new string?[] { null, ",", "from", "where", "group by", "having", "order by", "union" };

        var v = ValueParser.Parse(r);
        var isasc = true;

        if (r.PeekToken().AreContains(breaktokens))
        {
            return new SortableItem(v);
        }

        if (r.PeekToken().AreEqual("asc"))
        {
            r.ReadToken(); // read 'asc' token
            isasc = true;

        }
        else if (r.PeekToken().AreEqual("desc"))
        {
            r.ReadToken(); // read 'desc' token
            isasc = false;
        }

        if (r.PeekToken().AreEqual("nulls first"))
        {
            r.ReadToken();
            return new SortableItem(v, isasc, NullSortType.First);
        }
        else if (r.PeekToken().AreEqual("nulls last"))
        {
            r.ReadToken();
            return new SortableItem(v, isasc, NullSortType.Last);
        }
        return new SortableItem(v, isasc, NullSortType.Undefined);
    }
}