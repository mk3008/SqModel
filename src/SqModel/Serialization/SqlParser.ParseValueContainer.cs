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
    public LogicalExpression ParseValueContainer(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"{nameof(ParseValueContainer)}  start");

        var c = new LogicalExpression();

        c.Left = ParseValueClause(includeCurrentToken);
        var sign = string.Empty;
        if (CurrentToken.IsNotEmpty() && CurrentToken.First().IsSymbol())
        {
            sign = CurrentToken;
        }
        else
        {
            sign = ReadToken();
        }
        c.Right = ParseValueClause();
        c.Right.Conjunction = sign;

        Logger?.Invoke($"{nameof(ParseValueContainer)}  end : {c.ToQuery().CommandText}");
        return c;
    }
}
