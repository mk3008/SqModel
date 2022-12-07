using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public abstract class ValueBase : IQueryCommand, IQueryParameter
{
    public abstract string GetCurrentCommandText();

    public abstract IDictionary<string, object?> GetCurrentParameters();

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

    public IDictionary<string, object?> GetParameters()
    {
        var prm = GetCurrentParameters();
        prm = prm.Merge(OperatableValue!.GetParameters());
        return prm;
    }
}