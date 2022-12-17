using Carbunql.Analysis.Extensions;
using Carbunql.Core;
using Carbunql.Core.Clauses;

namespace Carbunql.Analysis.Parser;

public static class RelationParser
{
    public static Relation Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static Relation Parse(TokenReader r)
    {
        var tp = ParseRelationType(r);
        var table = SelectableTableParser.Parse(r);
        if (tp == RelationType.Cross) return new Relation(table, tp);

        r.ReadToken("on");
        var val = ValueParser.Parse(r);

        return new Relation(table, tp, val);
    }

    private static RelationType ParseRelationType(TokenReader r)
    {
        var tp = r.ReadToken(new string[] { "inner", "left", "right", "cross" });

        if (tp.AreEqual("inner join"))
        {
            return RelationType.Inner;
        }
        else if (tp.AreEqual("left join") || tp.AreEqual("left outer join"))
        {
            return RelationType.Left;
        }
        else if (tp.AreEqual("right join") || tp.AreEqual("right outer join"))
        {
            return RelationType.Right;
        }
        else if (tp.AreEqual("cross join"))
        {
            return RelationType.Cross;
        }
        throw new NotSupportedException();
    }
}