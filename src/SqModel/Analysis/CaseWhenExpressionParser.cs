using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Expression;

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
        var c = new CaseWhenExpression();

        var token = ReadUntilSplitToken();

        while (true)
        {
            if (Parser.CurrentToken.ToLower() == "when" || Parser.CurrentToken.ToLower() == "else")
            {
                token = ReadUntilSplitToken();
                continue;
            }

            if (Parser.CurrentToken.ToLower() == "then")
            {
                var cv = CreateCaseWhenValuePair(token);
                c.Collection.Add(cv);
                token = ReadUntilSplitToken();
                continue;
            }

            if (Parser.CurrentToken.ToLower() == "end")
            {
                //set ReturnValue
                var cv = CreateCaseWhenElseValue(token);
                c.Collection.Add(cv);
            }

            if (breakTokens.Contains(Parser.CurrentToken)) break;

            throw new SyntaxException("case when expression parse error.");
        }

        Parser.ReadToken();
        return c;
    }

    private CaseWhenValuePair CreateCaseWhenValuePair(string token)
    {
        //set Condition
        var cv = new CaseWhenValuePair();
        using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.When(p.ParseLogicalExpression());

        //set ReturnValue
        var valuetoken = ReadUntilSplitToken();
        using (var p = new SqlParser(valuetoken) { Logger = Parser.Logger }) cv.Then(p.ParseValueClause());

        return cv;
    }

    private CaseWhenValuePair CreateCaseWhenElseValue(string token)
    {
        //set ReturnValue
        var cv = new CaseWhenValuePair();
        using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.Then(p.ParseValueClause());

        return cv;
    }
}