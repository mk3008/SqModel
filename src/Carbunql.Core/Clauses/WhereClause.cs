namespace Carbunql.Core.Clauses;

public class WhereClause : IQueryCommand
{
    public WhereClause(ValueBase condition)
    {
        Condition = condition;
    }

    public ValueBase Condition { get; init; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, "where", BlockType.Start, true);
        foreach (var item in Condition.GetTokens()) yield return item;
        yield return (tp, string.Empty, BlockType.End, true);
    }
}