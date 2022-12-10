using SqModel.Expression;
using SqModel.Extension;

namespace SqModel.Analysis;

internal static class CaseWhenExpressionParser
{
    private static List<string> splitTokens = new() { "when", "then", "else", "end" };

    private static List<string> breakTokens = new() { "", "end" };

    private static string ReadUntilSplitToken(SqlParser parser)
        => parser.ReadUntilTokens(splitTokens, "case", "end");

    public static CaseWhenExpression ParseExpression(SqlParser parser)
    {
        /*
         * case  
         *    when Condition then Value 
         *    when Condition then Value
         *    else Value
         *  end
         */

        if (parser.CurrentToken != "when") throw new InvalidProgramException();

        var c = new CaseWhenExpression();
        var q = parser.ReadTokensWithoutComment();

        var fn = () =>
        {
            if (parser.CurrentToken.ToLower() == "else")
            {
                var value = ReadUntilSplitToken(parser);
                if (parser.CurrentToken.ToLower() != "end") throw new InvalidProgramException();

                //set ReturnValue
                var cv = new CaseWhenValuePair();
                cv.Then(ValueClauseParser.Parse(value));

                c.Collection.Add(cv);
                return;
            }

            if (parser.CurrentToken.ToLower() == "when")
            {
                var condition = ReadUntilSplitToken(parser);
                if (parser.CurrentToken.ToLower() != "then") throw new InvalidProgramException();

                //set Condition
                var cv = new CaseWhenValuePair();
                cv.When(LogicalExpressionParser.Parse(condition));

                //set ReturnValue
                var valuetoken = ReadUntilSplitToken(parser);
                cv.Then(ValueClauseParser.Parse(valuetoken));

                c.Collection.Add(cv);
                return;
            }

            throw new InvalidProgramException();
        };

        while (parser.CurrentToken.ToLower() != "end" && parser.CurrentToken.IsNotEmpty())
        {
            fn();
        };

        if (parser.CurrentToken.ToLower() == "end") q.First();
        return c;
    }
}