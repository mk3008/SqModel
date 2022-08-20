using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public OperatorContainer ParseOperatorContainer()
    {
        Logger?.Invoke($"ParseOperatorContainer start");

        var container = new OperatorContainer();
        var token = ReadToken();
        var @operator = string.Empty;
        var suboperator = string.Empty;

        while (token != String.Empty)
        {
            if (ConditionBreakTokens.Any(token)) break;
            if (token == ")") break;

            if (token.IsLogicalOperator())
            {
                @operator = token.ToLower();
                token = ReadToken();
                continue;
            }

            if (token.ToLower() == "not")
            {
                suboperator = token.ToLower();
                token = ReadToken();
                continue;
            }

            if (token.ToLower() == "exists")
            {
                while (token != "(" || token == null) token = ReadToken();
                if (token == null) break;

                using var p = new SqlParser(ReadUntilCloseBracket());
                p.Logger = Logger;
                var eq = p.ParseSelectQuery();
                container.Where().SetOperator(@operator, suboperator).Exists(eq);
            }
            else if (token == "(")
            {
                using var p = new SqlParser(ReadUntilCloseBracket());
                p.Logger = Logger;
                var c = p.ParseOperatorContainer().SetOperator(@operator, suboperator);
                container.ConditionGroup ??= new();
                container.ConditionGroup.Add(c);
            }
            else
            {
                container.Where().SetOperator(@operator, suboperator).Condition = ParseValueContainer(true);
                token = CurrentToken;
                continue;
            }

            @operator = string.Empty;
            suboperator = string.Empty;
            token = ReadToken();
        }

        Logger?.Invoke($"ParseOperatorContainer end : {container.ToQuery().CommandText}");
        return container;
    }
}
