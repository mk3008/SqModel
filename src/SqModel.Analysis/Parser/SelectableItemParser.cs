using SqModel.Analysis.Extensions;
using SqModel.Core.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis.Parser;

public static class SelectableItemParser
{
    public static SelectableItem Parse(string text)
    {
        using var r = new TokenReader(text);
        return Parse(r);
    }

    public static SelectableItem Parse(TokenReader r)
    {
        var breaktokens = new string?[] { null, ",", "from", "where", "group by", "having", "order by", "union" };

        var v = ValueParser.Parse(r);

        if (r.PeekToken().AreContains(breaktokens))
        {
            return new SelectableItem(v, v.GetDefaultName());
        }

        if (r.PeekToken().AreEqual("as"))
        {
            r.ReadToken(); // read 'as' token
            if (r.PeekToken().AreContains(breaktokens))
            {
                throw new SyntaxException($"alias name is not found.");
            }
        }

        return new SelectableItem(v, r.ReadToken());
    }
}