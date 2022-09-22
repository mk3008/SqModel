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

        AddGroup(parser, group, string.Empty);

        while (parser.CurrentToken.IsLogicalOperator())
        {
            var @operator = parser.CurrentToken;
            q.First();
            AddGroup(parser, group, @operator);
        }
        return group;
    }

    private static void AddGroup(SqlParser Parser, ConditionGroup group, string @operator)
    {
        var suboperator = string.Empty;

        var q = Parser.ReadTokensWithoutComment();

        if (Parser.CurrentToken.ToLower() == "not")
        {
            suboperator = Parser.CurrentToken;
            q.First();
        }

        if (Parser.CurrentToken == "(")
        {
            var g = new ConditionGroup() { Operator = @operator , SubOperator = suboperator};
            var inner = q.First(); // inner text
            q.First(); // ')'
            q.First(); // next

            using var p = new SqlParser(inner) { Logger = Parser.Logger };
            group.Collection.Add(g);

            AddGroup(p, g, @operator);
            while (p.CurrentToken.IsLogicalOperator())
            {
                var op = p.CurrentToken;
                p.ReadTokensWithoutComment().First();
                AddGroup(p, g, op);
            }

            return;
        }
        else if (Parser.CurrentToken.ToLower() == "exists")
        {
            var tmp = q.First();
            if (tmp != "(") throw new InvalidProgramException();

            var subquery = q.First(); // inner text
            q.First(); // ')'
            q.First(); // next

            using var p = new SqlParser(subquery) { Logger = Parser.Logger };
            var eq = p.ParseSelectQuery();
            eq.IsOneLineFormat = true;
            group.Add().SetOperator(@operator, suboperator).Exists(eq);
            return;
        }
        else
        {
            group.Add().SetOperator(@operator, suboperator).Expression = LogicalExpressionParser.Parse(Parser);
            return;
        }
    }
}
