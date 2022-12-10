using SqModel.Extension;

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

        c.Left = ValueClauseParser.Parse(parser);
        var sign = string.Empty;
        if (parser.CurrentToken.IsNotEmpty())
        {
            sign = parser.CurrentToken;
            q.First();

            c.Right = ValueClauseParser.Parse(parser);
            c.Right.Conjunction = sign;
        }
        return c;
    }
}
