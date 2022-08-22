using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public class CaseExpression<T> : CaseExpression where T : ConditionValuePair, new()
{
    internal T? CurrentConditionValuePair { get; set; } = null;

    public virtual T Add()
    {
        var c = new T();
        ConditionValues.Add(c);
        CurrentConditionValuePair = c;
        return c;
    }
}
