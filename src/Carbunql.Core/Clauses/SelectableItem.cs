namespace Carbunql.Core.Clauses;

public class SelectableItem : IQueryCommand, ISelectable
{
    public SelectableItem(ValueBase value, string alias)
    {
        Value = value;
        Alias = alias;
    }

    public ValueBase Value { get; init; }

    public string Alias { get; init; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        foreach (var item in Value.GetTokens()) yield return item;
        if (!string.IsNullOrEmpty(Alias) && Alias != Value.GetDefaultName())
        {
            yield return (tp, "as", BlockType.Default, true);
            yield return (tp, Alias, BlockType.Default, false);
        }
    }
}