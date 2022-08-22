using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public class CaseWhenConditionValuePair : ConditionValuePair
    , IWhenValueSettable<ISignValueClauseSettable<IReturnValueSettable>>
    , ISignValueClauseSettable<IReturnValueSettable>
{
    public ISignValueClauseSettable<IReturnValueSettable> SetWhenValueClause(ValueClause value)
    {
        ConditionExpression ??= new();
        ConditionExpression.Source = value;
        return this;
    }

    public IReturnValueSettable SetSignValueClause(string sign, ValueClause value)
    {
        if (ConditionExpression == null) throw new InvalidProgramException();

        var c = new ValueConjunction() { Sign = sign };
        ConditionExpression.ValueConjunction = c;
        c.Destination = value;

        return this;
    }
}