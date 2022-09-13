using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public static class WhereClauseParser
{
    private static string StartToken = "where";

    public static WhereClause Parse(string text)
    {
        using var p = new SqlParser(text);
        return Parse(p);
    }

    public static WhereClause Parse(SqlParser parser)
    {
        var w = new WhereClause();
        w.ConditionGroup = ParseConditionGroup(parser);
        w.ConditionGroup.IsDecorateBracket = false;
        w.ConditionGroup.IsOneLineFormat = false;
        return w;
    }

    private static ConditionGroup ParseConditionGroup(SqlParser parser)
    {
        var group = new ConditionGroup();
        var q = parser.ReadTokensWithoutComment();

        var startToken = parser.CurrentToken.IsNotEmpty() ? parser.CurrentToken : q.First();

        if (startToken.ToLower() != StartToken) throw new InvalidProgramException($"First token must be '{StartToken}'.");
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
            var subquery = Parser.ReadUntilCloseBracket();
            using var p = new SqlParser(subquery) { Logger = Parser.Logger };
            AddGroup(p, group, @operator);
            return;
        }
        else if (Parser.CurrentToken.ToLower() == "exists")
        {
            var tmp = q.First();
            if (tmp != "(") throw new InvalidProgramException();

            var subquery = Parser.ReadUntilCloseBracket();
            using var p = new SqlParser(subquery) { Logger = Parser.Logger };
            var eq = p.ParseSelectQuery();
            eq.IsOneLineFormat = true;
            group.Add().SetOperator(@operator, suboperator).Exists(eq);
            return;
        }
        else
        {
            group.Add().SetOperator(@operator, suboperator).Expression = Parser.ParseLogicalExpression();
            return;
        }
    }
}