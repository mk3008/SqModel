using SqModel;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Analysis;

public partial class SqlParser
{
    public LogicalExpression ParseLogicalExpression(bool includeCurrentToken = false)
    {
        Logger?.Invoke($"{nameof(ParseLogicalExpression)}  start");

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

        Logger?.Invoke($"{nameof(ParseLogicalExpression)}  end : {c.ToQuery().CommandText}");
        return c;
    }
}
