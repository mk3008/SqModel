using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public class CaseConditionValuePair : ConditionValuePair
    , IWhenValueSettable<IReturnValueSettable>
{
    public IReturnValueSettable SetWhenValueClause(ValueClause value)
    {
        ConditionValue = value;
        return this;
    }
}