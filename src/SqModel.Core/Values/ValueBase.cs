using SqModel.Core.Clauses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Values;

public abstract class ValueBase : IValue
{
    public abstract string GetCurrentCommandText();

    public virtual string GetDefaultName() => string.Empty;

    public string GetCommandText()
    {
        if (OperatableValue == null) return GetCurrentCommandText();
        return $"{GetCurrentCommandText()} {OperatableValue.GetCommandText()}";
    }

    public OperatableValue<ValueBase>? OperatableValue { get; private set; }

    public ValueBase? Inner { get; init; }

    public ValueBase AddOperatableValue(string @operator, ValueBase value)
    {
        if (OperatableValue != null) throw new InvalidOperationException();
        OperatableValue = new OperatableValue<ValueBase>(@operator, value);
        return value;
    }
}
