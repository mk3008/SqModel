using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class UnionClauseParser
{
    private static string StartToken = "union";

    public static UnionClause Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static UnionClause Parse(SqlParser parser)
    {
        var c = new UnionClause();
        var q = parser.ReadTokensWithoutComment();

        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new InvalidProgramException($"First token must be '{StartToken}'.");
        q.First(); //skip start token token

        if (parser.CurrentToken.ToLower() == "all")
        {
            c.IsUnionAll = true;
            q.First(); //skil 'all' token
        }
        else
        {
            c.IsUnionAll = false;
        }

        if (parser.CurrentToken != "select") throw new SyntaxException($"union clause syntax error.");
        c.SelectQuery = parser.ParseSelectQuery();

        return c;
    }
}