using SqModel.Building;
using SqModel.CommandContainer;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Serialization;

public partial class SqlParser
{
    public ConditionGroup ParseConditionGroup()
    {
        Logger?.Invoke($"{this.GetType().Name} start");

        var container = new ConditionGroup();
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

                using var p = new SqlParser(ReadUntilCloseBracket()) { Logger = Logger };
                var eq = p.ParseSelectQuery();
                eq.IsOneLineFormat = true;
                container.Add().SetOperator(@operator, suboperator).Exists(eq);
            }
            else if (token == "(")
            {
                using var p = new SqlParser(ReadUntilCloseBracket()) { Logger = Logger };
                var c = p.ParseConditionGroup().SetOperator(@operator, suboperator);
                container.Collection.Add(c);
            }
            else
            {
                container.Add().SetOperator(@operator, suboperator).Expression = ParseValueContainer(true);
                //token = CurrentToken;
                //continue;
            }

            @operator = string.Empty;
            suboperator = string.Empty;
            token = ReadToken();
        }

        Logger?.Invoke($"{this.GetType().Name}  end : {container.ToQuery().CommandText}");
        return container;
    }
}
