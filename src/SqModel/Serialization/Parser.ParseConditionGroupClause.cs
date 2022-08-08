using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public ConditionGroupClause ParseConditionGroup()
    {
        Logger?.Invoke($"ParseConditionGroup start");

        var g = new ConditionGroupClause();

        var token = ReadToken();
        var operatorToken = string.Empty;

        while (token != String.Empty)
        {
            if (ConditionBreakTokens.Any(token)) break;
            if (token == ")") break;

            if (token.IsLogicalOperator())
            {
                operatorToken = token.ToLower();
                token = ReadToken();
                continue;
            }

            if (token == "(")
            {
                using var p = new Parser(ReadUntilCloseBracket());
                p.Logger = Logger;
                var c = p.ParseConditionGroup();
                c.Operator = operatorToken;
                g.GroupConditions.Add(c);
            }
            else
            {
                var c = ParseCondition(true);
                c.Operator = operatorToken;
                g.Conditions.Add(c);
                token = CurrentToken;

                if (token.IsLogicalOperator())
                {
                    operatorToken = token.ToLower();
                    token = ReadToken();
                    continue;
                }
                continue;
            }

            operatorToken = string.Empty;
            token = ReadToken();
        }

        Logger?.Invoke($"ParseConditionGroup end : {g.ToQuery().CommandText}");
        return g;
    }
}
