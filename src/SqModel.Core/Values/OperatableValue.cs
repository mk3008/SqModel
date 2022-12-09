using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class OperatableValue : IQueryCommand, IQueryParameter
{
    public OperatableValue(string @operator, ValueBase value)
    {
        Operator = @operator;
        Value = value;
    }

    public string Operator { get; init; }

    public ValueBase Value { get; init; }

    public string GetCommandText()
    {
        if (string.IsNullOrEmpty(Operator)) return $"{Value.GetCommandText()}";
        return $"{Operator} {Value.GetCommandText()}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Value.GetParameters();
    }
}