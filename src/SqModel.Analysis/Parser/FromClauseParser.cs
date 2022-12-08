using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;

namespace SqModel.Analysis.Parser;

public static class FromClauseParser
{
    public static FromClause Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static FromClause Parse(TokenReader r)
    {
        var relationtokens = new string?[] { "inner join", "left join", "left outer join", "right join", "right outer join", "cross join" };

        var root = SelectableTableParser.Parse(r);
        var from = new FromClause(root);

        if (!r.PeekToken().AreContains(relationtokens))
        {
            return from;
        }
        from.Relations ??= new List<Relation>();

        do
        {
            from.Relations.Add(RelationParser.Parse(r));

        } while (r.PeekToken().AreContains(relationtokens));

        return from;
    }
}