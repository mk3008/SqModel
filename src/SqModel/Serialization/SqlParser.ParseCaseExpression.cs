using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public CaseExpression ParseCaseExpression(bool includeCurrentToken = false)
    {
        var h = new CaseExpressionParseHelper(this);

        var token = (includeCurrentToken) ? CurrentToken : ReadToken();
        var next = ReadToken();
        if (token.ToLower() == "case" && next.ToLower() == "when") return h.ParseCaseWhen();
        if (token.ToLower() == "case" ) return h.ParseCase();
        throw new SyntaxException("case expression parse error.");
    }

    internal class CaseExpressionParseHelper
    {
        public CaseExpressionParseHelper(SqlParser parser)
        {
            Parser = parser;
        }

        public SqlParser Parser { get; init; }
        private static List<string> splitTokens = new() { "when", "then", "else", "end" };
        private static List<string> breakTokens = new() { "", "end" };

        private string ReadUntilSplitToken() => Parser.ReadUntilTokens(splitTokens, "case", "end");

        public CaseExpression ParseCase()
        {
            /*
             * case CaseValue
             *    when ConditionValue then ReturnValue
             *    when ConditionValue then ReturnValue
             *    else ReturnValue
             * end
             */
            var w = new CaseExpression();

            //CaseValue
            w.Value = ParseCaseValue();

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
                    var cv = CreateValueValuePair(token);
                    w.ConditionValues.Add(cv);
                    token = ReadUntilSplitToken();
                    continue;
                }

                if (Parser.CurrentToken.ToLower() == "end")
                {
                    var cv = CreateElseValue(token);
                    w.ConditionValues.Add(cv);
                }

                if (breakTokens.Contains(Parser.CurrentToken)) break;

                throw new SyntaxException("case expression parse error.");
            }

            return w;
        }

        public CaseExpression ParseCaseWhen()
        {
            /*
             * case  
             *    when Condition then ReturnValue 
             *    when Condition then ReturnValue
             *    else ReturnValue
             *  end
             */
            var w = new CaseExpression();

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
                    var cv = CreateConditionValuePair(token);
                    w.ConditionValues.Add(cv);
                    token = ReadUntilSplitToken();
                    continue;
                }

                if (Parser.CurrentToken.ToLower() == "end")
                {
                    //set ReturnValue
                    var cv = CreateElseValue(token);
                    w.ConditionValues.Add(cv);
                }

                if (breakTokens.Contains(Parser.CurrentToken)) break;

                throw new SyntaxException("case when expression parse error.");
            }

            return w;
        }

        private ValueClause ParseCaseValue()
        {
            var text = $"{Parser.CurrentToken}{Parser.ReadUntilTokens(new() { "when" })}";
            using var p = new SqlParser(text) { Logger = Parser.Logger };
            if (Parser.CurrentToken != "when") throw new InvalidOperationException();
            return p.ParseValueClause();
        }

        private ConditionValuePair CreateValueValuePair(string token)
        {
            //set ConditionValue
            var cv = new ConditionValuePair();
            using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.ConditionValue = p.ParseValueClause();

            //set ReturnValue
            var valuetoken = ReadUntilSplitToken();
            using (var p = new SqlParser(valuetoken) { Logger = Parser.Logger }) cv.ReturnValue = p.ParseValueClause();

            return cv;
        }

        private ConditionValuePair CreateConditionValuePair(string token)
        {
            //set Condition
            var cv = new ConditionValuePair();
            using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.ConditionExpression = p.ParseValueContainer();

            //set ReturnValue
            var valuetoken = ReadUntilSplitToken();
            using (var p = new SqlParser(valuetoken) { Logger = Parser.Logger }) cv.ReturnValue = p.ParseValueClause();

            return cv;
        }

        private ConditionValuePair CreateElseValue(string token)
        {
            //set ReturnValue
            var cv = new ConditionValuePair();
            using (var p = new SqlParser(token) { Logger = Parser.Logger }) cv.ReturnValue = p.ParseValueClause();

            return cv;
        }
    }
}
