using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public class WhenExpression
{
    public WhenExpression(ValueBase condition, ValueBase value)
    {
        Condition = condition;
        Value = value;
    }

    public ValueBase Condition { get; init; }

    public ValueBase Value { get; init; }

    public string GetCommandText()
    {
        return "when " + Condition.GetCommandText() + " then " + Value.GetCommandText();
    }
}