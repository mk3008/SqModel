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

    public IEnumerable<Token> GetTokens(Token? parent)
    {
        foreach (var item in Value.GetTokens(parent)) yield return item;
        if (!IsAscending) yield return Token.Reserved(this, parent, "dest");
        if (NullSort != NullSortType.Undefined) yield return Token.Reserved(this, parent, NullSort.ToCommandText());
    }
}