using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class OperatableValue<T> : IQueryCommand where T : IValue
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
        if (Operator.AreEqual("over")) return $"{Operator}{Value.GetCommandText()}";
        return $"{Operator} {Value.GetCommandText()}";
    }
}