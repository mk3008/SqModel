using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqModel.Analysis;

internal static class RelationGroupParser
{
    private static string StartToken = "on";

    public static RelationGroup Parse(SqlParser parser)
    {
        var group = new RelationGroup() { IsDecorateBracket = false };

        var q = parser.ReadTokensWithoutComment();
        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new InvalidProgramException($"First token must be '{StartToken}'.");
        q.First(); //skip start token token

        AddRelation(parser, group, string.Empty);

        while (parser.CurrentToken.IsLogicalOperator())
        {
            var @operator = parser.CurrentToken;

            q.First();

            AddRelation(parser, group, @operator);
        }
        return group;
    }

    public static void AddRelation(SqlParser parser, RelationGroup group, string @operator)
    {
        var suboperator = string.Empty;

        var q = parser.ReadTokensWithoutComment();

        if (parser.CurrentToken.ToLower() == "not")
        {
            suboperator = parser.CurrentToken;
            q.First();
        }

        if (parser.CurrentToken == "(")
        {
            var subquery = q.First(); //inner text
            q.First(); // ')'
            q.First(); // next

            using var p = new SqlParser(subquery) { Logger = parser.Logger };
            AddRelation(p, group, @operator);
            return;
        }
        else if (parser.CurrentToken.ToLower() == "exists")
        {
            var tmp = q.First();
            if (tmp != "(") throw new InvalidProgramException();

            var subquery = q.First(); //inner text
            q.First(); // ')'
            q.First(); // next

            using var p = new SqlParser(subquery) { Logger = parser.Logger };
            var eq = p.ParseSelectQuery();
            eq.IsOneLineFormat = true;
            group.Add().SetOperator(@operator, suboperator).Exists(eq);
            return;
        }
        else
        {
            group.Add().SetOperator(@operator, suboperator).Expression = LogicalExpressionParser.Parse(parser);
            return;
        }
    }
}