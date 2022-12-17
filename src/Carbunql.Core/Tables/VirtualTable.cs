using Carbunql.Core.Clauses;

namespace Carbunql.Core.Tables;

public class VirtualTable : TableBase
{
    public VirtualTable(IQueryCommandable query)
    {
        Query = query;
    }

    public IQueryCommandable Query { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, "(", BlockType.Start, true);
        foreach (var item in Query.GetTokens()) yield return item;
        yield return (tp, ")", BlockType.End, true);
    }
}