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

    public WhenExpression(ValueBase value)
    {
        Value = value;
    }

    public ValueBase? Condition { get; init; }

    public ValueBase Value { get; init; }

    public string GetCommandText()
    {
        if (Condition != null) return "when " + Condition.GetCommandText() + " then " + Value.GetCommandText();
        return "else " + Value.GetCommandText();
    }
}