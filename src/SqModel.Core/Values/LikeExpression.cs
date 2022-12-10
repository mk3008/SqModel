using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class LikeExpression : ValueBase
{
    public LikeExpression(ValueBase value, ValueBase argument)
    {
        Value = value;
        Argument = argument;
    }

    public ValueBase Value { get; init; }

    public ValueBase Argument { get; init; }

    public override string GetCurrentCommandText()
    {
        return Value.GetCommandText() + " like " + Argument.GetCommandText();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = Value.GetParameters();
        prm = prm.Merge(Argument.GetParameters());
        return prm;
    }
}