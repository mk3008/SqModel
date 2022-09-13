using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class LogicalExpressionParser
{
    public static LogicalExpression Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static LogicalExpression Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();

        var c = new LogicalExpression();

        c.Left = parser.ParseValueClause();
        var sign = string.Empty;
        if (parser.CurrentToken.IsNotEmpty())
        {
            sign = parser.CurrentToken;
            q.First();
        }
        else
        {
            throw new SyntaxException("LogicalExpression syntax error.");
        }

        c.Right = parser.ParseValueClause();
        c.Right.Conjunction = sign;

        return c;
    }
}
