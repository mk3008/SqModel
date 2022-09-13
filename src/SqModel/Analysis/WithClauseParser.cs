using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

internal static class WithClauseParser
{
    private static string StartToken = "with";

    public static WithClause Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();
        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new SyntaxException($"First token must be '{StartToken}'.");
        q.First(); //skip start token token

        var w = new WithClause();

        w.Collection.Add(ParseSelecItem(parser));

        while (parser.CurrentToken == ",")
        {
            q.First();//skip ',' token
            w.Collection.Add(ParseSelecItem(parser));
        }

        return w;
    }

    private static CommonTable ParseSelecItem(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();

        var c = new CommonTable();

        c.Name = parser.CurrentToken;
        q.First();
        if (parser.CurrentToken.ToLower() == "as") q.First(); // skip 'as' token

        if (parser.CurrentToken != "(") throw new SyntaxException("with clauese syntax error.");
        q.First(); // skip '(' token

        c.Query = SqlParser.Parse(parser.CurrentToken);

        q.First(); // skip sqltext token

        if (parser.CurrentToken != ")") throw new SyntaxException("with clauese syntax error.");

        q.First(); // skip ')' token

        return c;
    }
}
