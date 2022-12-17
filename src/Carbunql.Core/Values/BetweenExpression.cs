using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class BetweenExpression : ValueBase
{
    public BetweenExpression(ValueBase value, ValueBase start, ValueBase end)
    {
        Value = value;
        Start = start;
        End = end;
    }

    public ValueBase Value { get; init; }

    public ValueBase Start { get; init; }

    public ValueBase End { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        foreach (var item in Value.GetCurrentTokens()) yield return item;
        yield return (tp, "between", BlockType.Default, true);
        foreach (var item in Start.GetCurrentTokens()) yield return item;
        yield return (tp, "and", BlockType.Default, true);
        foreach (var item in End.GetCurrentTokens()) yield return item;
    }
}