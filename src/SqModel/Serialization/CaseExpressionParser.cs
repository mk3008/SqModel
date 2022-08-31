using SqModel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

internal class CaseExpressionParser
{
    public CaseExpressionParser(SqlParser parser)
    {
        Parser = parser;
    }

    private SqlParser Parser { get; init; }
    private static List<string> splitTokens = new() { "when", "then", "else", "end" };
    private static List<string> breakTokens = new() { "", "end" };

    private string ReadUntilSplitToken() => Parser.ReadUntilTokens(splitTokens, "case", "end");

    public CaseExpression Parse()
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
        c.Value = ParseCaseValue();

        var token = ReadUntilSplitToken();

        while (true)
        {
            if (Parser.CurrentToken.ToLower() == "when" || Parser.CurrentToken.ToLower() == "else")
            {
                token = ReadUntilSplitToken();
                continue;
            };

            if (Parser.CurrentToken.ToLower() == "then")
            {
                var cv = CreateCaseValuePair(token);
                c.Collection.Add(cv);
                token = ReadUntilSplitToken();
                continue;
            }

            if (Parser.CurrentToken.ToLower() == "end")
            {
                var cv = CreateCaseElseValue(token);
                c.Collection.Add(cv);
            }

            if (breakTokens.Contains(Parser.CurrentToken)) break;

            throw new SyntaxException("case expression parse error.");
        }

        Parser.ReadToken();
        return c;
    }

    private ICommand ParseCaseValue()
    {
        var text = $"{Parser.CurrentToken}{Parser.ReadUntilTokens(new() { "when" })}";
        using var p = new SqlParser(text) { Logger = Parser.Logger };
        if (Parser.CurrentToken != "when") throw new InvalidOperationException();
        return p.ParseValueClause();
    }

    private CaseValuePair CreateCaseValuePair(string token)
    {
        //set ConditionValue
        var cv = new CaseValuePair();
        using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.When(p.ParseValueClause());

        //set ReturnValue
        var valuetoken = ReadUntilSplitToken();
        using (var p = new SqlParser(valuetoken) { Logger = Parser.Logger }) cv.Then(p.ParseValueClause());

        return cv;
    }

    private CaseValuePair CreateCaseElseValue(string token)
    {
        //set ReturnValue
        var cv = new CaseValuePair();
        using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.Then(p.ParseValueClause());

        return cv;
    }
}