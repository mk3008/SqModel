using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

internal static class SelectClauseParser
{
    private static string StartToken = "select";

    public static SelectClause Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();
        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new SyntaxException($"First token must be '{StartToken}'.");
        q.First(); //skip start token token

        var sc = new SelectClause();

        if (parser.CurrentToken.ToLower() == "distinct")
        {
            sc.Distinct();
            q.First(); //skip distinct token token
        }

        sc.Collection.Add(ParseSelecItem(parser));

        while (parser.CurrentToken == ",")
        {
            q.First();//skip ',' token
            sc.Collection.Add(ParseSelecItem(parser));
        }
        return sc;
    }

    public static SelectItem ParseSelecItem(SqlParser parser)
    {
        var c = new SelectItem();
        c.Command = ValueClauseParser.Parse(parser);

        if (!parser.AliasBreakTokens.Contains(parser.CurrentToken))
        {
            c.Name = parser.ParseAlias();
        }
        if (c.Name.IsEmpty())
        {
            var n = c.Command.GetName();
            if (n.IsLetter()) c.Name = c.Command.GetName();
        }

        return c;
    }
}
