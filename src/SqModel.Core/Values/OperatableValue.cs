using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class OperatableValue<T> : IQueryCommand, IQueryParameter where T : ValueBase
{
    public OperatableValue(string @operator, T value)
    {
        Operator = @operator;
        Value = value;
    }

    public string Operator { get; init; }

    public T Value { get; init; }

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