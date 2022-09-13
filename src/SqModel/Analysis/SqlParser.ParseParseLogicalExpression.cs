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
    public LogicalExpression ParseLogicalExpression()
    {
        Logger?.Invoke($"{nameof(ParseLogicalExpression)}  start");

        var c = new LogicalExpression();

        c.Left = ParseValueClause();
        var sign = string.Empty;
        if (CurrentToken.IsNotEmpty())
        {
            sign = CurrentToken;
            ReadTokensWithoutComment().First();
        }
        else
        {
            throw new InvalidProgramException();
        }

        c.Right = ParseValueClause();
        c.Right.Conjunction = sign;

        Logger?.Invoke($"{nameof(ParseLogicalExpression)}  end : {c.ToQuery().CommandText}");
        return c;
    }
}
