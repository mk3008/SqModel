using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class OrderClauseParser
{
    private static string StartToken = "order by";

    public static OrderClause Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static OrderClause Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();
        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new SyntaxException($"First token must be '{StartToken}'.");
        q.First(); //skip start token token

        var oc = new OrderClause();

        oc.Collection.Add(ParseOrderItem(parser));

        while (parser.CurrentToken == ",")
        {
            q.First();//skip ',' token
            oc.Collection.Add(ParseOrderItem(parser));
        }
        return oc;
    }

    public static OrderItem ParseOrderItem(SqlParser parser)
    {
        var c = new OrderItem();
        c.Command = ValueClauseParser.Parse(parser);
        return c;
    }
}
