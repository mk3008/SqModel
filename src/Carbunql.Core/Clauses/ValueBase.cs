using Carbunql.Core.Values;

namespace Carbunql.Core.Clauses;

public abstract class ValueBase : IQueryCommand
{
    public string? Sufix { get; set; }

    public virtual string GetDefaultName() => string.Empty;

    public OperatableValue? OperatableValue { get; private set; }

    public ValueBase AddOperatableValue(string @operator, ValueBase value)
    {
        if (OperatableValue != null) throw new InvalidOperationException();
        OperatableValue = new OperatableValue(@operator, value);
        return value;
    }

    public abstract IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens();

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        foreach (var item in GetCurrentTokens()) yield return item;

        if (Sufix != null) yield return (tp, Sufix, BlockType.Default, true);
        if (OperatableValue != null)
        {
            foreach (var item in OperatableValue.GetTokens()) yield return item;
        }
    }
}