using SqModel.Expression;
using SqModel.Extension;

namespace SqModel.Analysis;

public static class CaseExpressionParser
{
    private static List<string> splitTokens = new() { "when", "then", "else", "end" };

    private static List<string> breakTokens = new() { "", "end" };

    private static string ReadUntilSplitToken(SqlParser parser)
        => parser.ReadUntilTokens(splitTokens, "case", "end");

    public static IValueClause Parse(SqlParser parser)
    {
        var q = parser.ReadTokensWithoutComment();

        var token = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();
        var next = q.First();

        if (token.ToLower() == "case" && next.ToLower() == "when")
        {
            return CaseWhenExpressionParser.ParseExpression(parser);
        }
        if (token.ToLower() == "case")
        {
            return ParseExpression(parser);
        }
        throw new SyntaxException("case expression parse error.");
    }

    public static CaseExpression ParseExpression(SqlParser parser)
    {
        /*
         * case CaseValue
         *    when ConditionValue then ReturnValue
         *    when ConditionValue then ReturnValue
         *    else ReturnValue
         * end
         */
        var c = new CaseExpression();

        //CaseValue
        c.Value = ParseCaseValue(parser);

        var token = ReadUntilSplitToken(parser);

        while (true)
        {
            if (parser.CurrentToken.ToLower() == "when" || parser.CurrentToken.ToLower() == "else")
            {
                token = ReadUntilSplitToken(parser);
                continue;
            };

            if (parser.CurrentToken.ToLower() == "then")
            {
                var cv = CreateCaseValuePair(parser, token);
                c.Collection.Add(cv);

                if (parser.CurrentToken.ToLower() == "end") break;

                token = ReadUntilSplitToken(parser);
                continue;

            }

            if (parser.CurrentToken.ToLower() == "end")
            {
                var cv = CreateCaseElseValue(token);
                c.Collection.Add(cv);
            }

            if (breakTokens.Contains(parser.CurrentToken)) break;

            throw new SyntaxException("case expression parse error.");
        }

        parser.ReadToken();
        return c;
    }

    private static IValueClause ParseCaseValue(SqlParser parser)
    {
        var text = $"{parser.CurrentToken}{parser.ReadUntilTokens(new() { "when" })}";
        using var p = new SqlParser(text) { Logger = parser.Logger };
        if (parser.CurrentToken != "when") throw new InvalidOperationException();
        return ValueClauseParser.Parse(p);
    }

    private static CaseValuePair CreateCaseValuePair(SqlParser parser, string token)
    {
        //set ConditionValue
        var cv = new CaseValuePair();
        cv.When(ValueClauseParser.Parse(token));

        //set ReturnValue
        var valuetoken = ReadUntilSplitToken(parser);
        cv.Then(ValueClauseParser.Parse(valuetoken));
        return cv;
    }

    private static CaseValuePair CreateCaseElseValue(string token)
    {
        //set ReturnValue
        var cv = new CaseValuePair();
        cv.Then(ValueClauseParser.Parse(token));
        return cv;
    }
}