using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Expression;
using SqModel.Extension;

namespace SqModel.Analysis;

internal class CaseWhenExpressionParser
{
    public CaseWhenExpressionParser(SqlParser parser)
    {
        Parser = parser;
    }

    private SqlParser Parser { get; init; }

    private static List<string> splitTokens = new() { "when", "then", "else", "end" };

    private static List<string> breakTokens = new() { "", "end" };

    private string ReadUntilSplitToken() => Parser.ReadUntilTokens(splitTokens, "case", "end");

    public CaseWhenExpression Parse()
    {
        /*
         * case  
         *    when Condition then Value 
         *    when Condition then Value
         *    else Value
         *  end
         */

        if (Parser.CurrentToken != "when") throw new InvalidProgramException();

        var c = new CaseWhenExpression();
        var q = Parser.ReadTokensWithoutComment();

        var fn = () =>
        {
            if (Parser.CurrentToken.ToLower() == "else")
            {
                var value = ReadUntilSplitToken();
                if (Parser.CurrentToken.ToLower() != "end") throw new InvalidProgramException();

                //set ReturnValue
                var cv = new CaseWhenValuePair();
                using (var p = new SqlParser(value) { Logger = Parser.Logger })
                {
                    cv.Then(p.ParseValueClause());
                }

                c.Collection.Add(cv);
                return;
            }

            if (Parser.CurrentToken.ToLower() == "when")
            {
                var condition = ReadUntilSplitToken();
                if (Parser.CurrentToken.ToLower() != "then") throw new InvalidProgramException();

                //set Condition
                var cv = new CaseWhenValuePair();
                using (var p = new SqlParser(condition) { Logger = Parser.Logger })
                {
                    cv.When(LogicalExpressionParser.Parse(p));
                }

                //set ReturnValue
                var valuetoken = ReadUntilSplitToken();
                using (var p = new SqlParser(valuetoken) { Logger = Parser.Logger })
                {
                    cv.Then(p.ParseValueClause());
                }

                c.Collection.Add(cv);
                return;
            }

            throw new InvalidProgramException();
        };

        while (Parser.CurrentToken.ToLower() != "end" && Parser.CurrentToken.IsNotEmpty())
        {
            fn();
        };

        if (Parser.CurrentToken.ToLower() == "end") q.First();
        return c;
    }
}