using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class WhenExpression : IQueryCommand
{
    public WhenExpression(ValueBase condition, ValueBase value)
    {
        Condition = condition;
        Value = value;
    }

    public WhenExpression(ValueBase value)
    {
        Value = value;
    }

    public ValueBase? Condition { get; init; }

    public ValueBase Value { get; init; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        if (Condition != null)
        {
            yield return (tp, "when", BlockType.Default, true);
            foreach (var item in Condition.GetTokens()) yield return item;
            yield return (tp, "then", BlockType.Default, true);
            foreach (var item in Value.GetTokens()) yield return item;
        }
        else
        {
            yield return (tp, "else", BlockType.Default, true);
            foreach (var item in Value.GetTokens()) yield return item;
        }
    }
}