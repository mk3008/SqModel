using SqModel.Analysis.Builder;
using SqModel.Core.Clauses;
using SqModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Analysis.Extensions;

namespace SqModel.Analysis.Parser;

public static class RelationParser
{
    public static Relation Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static Relation Parse(TokenReader r)
    {
        var breaktokens = new string?[] { null, "inner join", "left join", "left outer join", "right join", "right outer join", "cross join", "where", "group by", "having", "order by", "union" };

        var tp = ParseRelationType(r);
        var table = SelectableTableParser.Parse(r);
        if (tp == RelationType.Cross) return new Relation(table, tp);

        if (!r.PeekToken().AreEqual("on")) throw new SyntaxException("not found 'on' token.");
        r.ReadToken();

        var val = ValueParser.Parse(r);

        return new Relation(table, tp, val);
    }

    private static RelationType ParseRelationType(TokenReader r)
    {
        var tp = r.ReadToken();

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