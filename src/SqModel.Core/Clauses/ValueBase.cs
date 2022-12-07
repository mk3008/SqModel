using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public abstract class ValueBase : IQueryCommand
{
    public abstract string GetCurrentCommandText();

    public virtual string GetDefaultName() => string.Empty;

    public string GetCommandText()
    {
        if (OperatableValue == null) return GetCurrentCommandText();
        return $"{GetCurrentCommandText()} {OperatableValue.GetCommandText()}";
    }

    public OperatableValue<ValueBase>? OperatableValue { get; private set; }

    public ValueBase AddOperatableValue(string @operator, ValueBase value)
    {
        if (OperatableValue != null) throw new InvalidOperationException();
        OperatableValue = new OperatableValue<ValueBase>(@operator, value);
        return value;
    }
}