using SqModel.Analysis.Builder;
using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Parser;

public static class SelectableTableParser
{
    public static SelectableTable Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectableTable Parse(TokenReader r)
    {
        var breaktokens = new string?[] { null, "inner join", "left join", "left outer join", "right join", "right outer join", "cross join", "where", "group by", "having", "order by", "union" };

        var v = TableParser.Parse(r);

        if (r.PeekToken().AreContains(breaktokens))
        {
            return new SelectableTable(v, v.GetDefaultName());
        }

        if (r.PeekToken().AreEqual("as"))
        {
            r.ReadToken(); // read 'as' token
            if (r.PeekToken().AreContains(breaktokens))
            {
                throw new SyntaxException($"alias name is not found.");
            }
        }

        var alias = r.ReadToken();

        if (!r.PeekToken().AreEqual("("))
        {
            return new SelectableTable(v, alias);
        }

        r.ReadToken();
        var (_, inner) = r.ReadUntilCloseBracket();
        var colAliases = ValueCollectionParser.Parse(inner);

        return new SelectableTable(v, alias, colAliases);
    }

}