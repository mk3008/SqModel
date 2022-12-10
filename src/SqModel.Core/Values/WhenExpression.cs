using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class WhenExpression : IQueryCommand, IQueryParameter
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

    public IDictionary<string, object?> GetParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Condition!.GetParameters());
        prm = prm.Merge(Value!.GetParameters());
        return prm;
    }
}