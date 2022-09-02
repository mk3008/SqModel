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
    public ConditionGroupParser(SqlParser parser)
    {
        Parser = parser;
    }

    private SqlParser Parser { get; init; }

    public ConditionGroup Parse()
    {
        var group = new ConditionGroup();
        var token = Parser.ReadToken();
        var @operator = string.Empty;
        var suboperator = string.Empty;

        while (token != string.Empty)
        {
            if (Parser.ConditionBreakTokens.Any(token)) break;
            if (token == ")") break;

            if (token.IsLogicalOperator())
            {
                @operator = token.ToLower();
                token = Parser.ReadToken();
                continue;
            }

            if (token.ToLower() == "not")
            {
                suboperator = token.ToLower();
                token = Parser.ReadToken();
                continue;
            }

            if (token.ToLower() == "exists")
            {
                while (token != "(" || token == null) token = Parser.ReadToken();
                if (token == null) break;

                using var p = new SqlParser(Parser.ReadUntilCloseBracket()) { Logger = Parser.Logger };
                var eq = p.ParseSelectQuery();
                eq.IsOneLineFormat = true;
                group.Add().SetOperator(@operator, suboperator).Exists(eq);
            }
            else if (token == "(")
            {
                using var p = new SqlParser(Parser.ReadUntilCloseBracket()) { Logger = Parser.Logger };
                var xp = new ConditionGroupParser(p);
                var c = xp.Parse().SetOperator(@operator, suboperator);
                group.Collection.Add(c);
            }
            else
            {
                group.Add().SetOperator(@operator, suboperator).Expression = Parser.ParseValueContainer(true);
                token = Parser.CurrentToken;
                continue;
            }

            @operator = string.Empty;
            suboperator = string.Empty;
            token = Parser.ReadToken();
        }

        return group;
    }
}
