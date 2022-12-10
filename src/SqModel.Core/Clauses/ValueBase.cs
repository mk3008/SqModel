using Cysharp.Text;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public abstract class ValueBase : IQueryCommand, IQueryParameter
{
    public abstract string GetCurrentCommandText();

    public string? Sufix { get; set; }

    public abstract IDictionary<string, object?> GetCurrentParameters();

    public virtual string GetDefaultName() => string.Empty;

    public string GetCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        sb.Append(GetCurrentCommandText());
        if (Sufix != null) sb.Append(Sufix);
        if (OperatableValue != null) sb.Append(" " + OperatableValue.GetCommandText());
        return sb.ToString();
    }

    public OperatableValue? OperatableValue { get; private set; }

    public ValueBase AddOperatableValue(string @operator, ValueBase value)
    {
        if (OperatableValue != null) throw new InvalidOperationException();
        OperatableValue = new OperatableValue(@operator, value);
        return value;
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm = GetCurrentParameters();
        prm = prm.Merge(OperatableValue!.GetParameters());
        return prm;
    }
}