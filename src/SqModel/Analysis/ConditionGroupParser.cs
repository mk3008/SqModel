using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

internal class ConditionGroupParser
{
    internal static ConditionGroup Parse(SqlParser parser, string starttoken)
    {
        var group = new ConditionGroup();
        var q = parser.ReadTokensWithoutComment();
        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != starttoken) throw new InvalidProgramException($"First token must be '{starttoken}'.");
        q.First(); //skip start token token

        var isFirst = true;
        while (!parser.ConditionBreakTokens.Contains(parser.CurrentToken) && parser.CurrentToken.IsNotEmpty())
        {
            var op = string.Empty;
            var sop = string.Empty;
            if (!isFirst)
            {
                op = parser.CurrentToken;
                q.First();
                if (parser.CurrentToken.ToLower() == "not")
                {
                    sop = parser.CurrentToken.ToLower();
                    q.First();
                }
            }
            else
            {
                isFirst = false;
            }
            var c = ValueClauseParser.ParseAsExpression(parser);
            c.Operator = op;
            c.SubOperator = sop;
            group.Collection.Add(c);
        }
        return group;
    }
}
