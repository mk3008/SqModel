using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class Parser
{
    public OperatorContainer ParseOperatorContainer()
    {
        Logger?.Invoke($"ParseOperatorContainer start");

        var container = new OperatorContainer();
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
                var c = p.ParseOperatorContainer();
                c.Operator = operatorToken;
                container.ConditionGroup ??= new();
                container.ConditionGroup.Add(c);
            }
            else
            {
                var c = new OperatorContainer();
                c.Operator = operatorToken;
                c.Condition = ParseValueContainer(true);

                container.ConditionGroup ??= new();
                container.ConditionGroup.Add(c);

                token = CurrentToken;
                continue;
            }

            operatorToken = string.Empty;
            token = ReadToken();
        }

        Logger?.Invoke($"ParseOperatorContainer end : {container.ToQuery().CommandText}");
        return container;
    }
}
