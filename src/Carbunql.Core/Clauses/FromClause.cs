namespace Carbunql.Core.Clauses;

public class FromClause : IQueryCommand
{
    public FromClause(SelectableTable root)
    {
        Root = root;
    }

    public SelectableTable Root { get; init; }

    public List<Relation>? Relations { get; set; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, "from", BlockType.Start, true);
        foreach (var item in Root.GetTokens()) yield return item;

        if (Relations != null)
        {
            foreach (var item in Relations)
            {
                yield return (tp, string.Empty, BlockType.Split, true);
                foreach (var token in item.GetTokens()) yield return token;
            }
        }
        yield return (tp, string.Empty, BlockType.End, true);
    }
}