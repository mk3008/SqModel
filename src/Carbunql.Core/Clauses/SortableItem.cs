using Carbunql.Core.Extensions;

namespace Carbunql.Core.Clauses;

public class SortableItem : IQueryCommand
{
    public SortableItem(ValueBase value, bool isAscending = true, NullSortType tp = NullSortType.Undefined)
    {
        Value = value;
        IsAscending = isAscending;
        NullSort = tp;
    }

    public ValueBase Value { get; init; }

    public bool IsAscending { get; set; } = true;

    public NullSortType NullSort { get; set; } = NullSortType.Undefined;

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        foreach (var item in Value.GetTokens()) yield return item;
        if (!IsAscending) yield return (tp, "dest", BlockType.Default, true);
        if (NullSort != NullSortType.Undefined) yield return (tp, NullSort.ToCommandText(), BlockType.Default, true);
    }
}