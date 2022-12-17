﻿using System.Collections;

namespace Carbunql.Core.Clauses;

public class OrderClause : IList<SortableItem>, IQueryCommand
{
    public OrderClause(List<SortableItem> orders)
    {
        Items.AddRange(orders);
    }

    private List<SortableItem> Items { get; init; } = new();

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, "order by", BlockType.Start, true);

        var isFirst = true;
        foreach (var item in Items)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return (tp, ",", BlockType.Split, true);
            }
            foreach (var token in item.GetTokens()) yield return token;
        }
        yield return (tp, string.Empty, BlockType.End, true);
    }

    #region implements IList<SortableItem>
    public SortableItem this[int index] { get => ((IList<SortableItem>)Items)[index]; set => ((IList<SortableItem>)Items)[index] = value; }

    public int Count => ((ICollection<SortableItem>)Items).Count;

    public bool IsReadOnly => ((ICollection<SortableItem>)Items).IsReadOnly;

    public void Add(SortableItem item)
    {
        ((ICollection<SortableItem>)Items).Add(item);
    }

    public void Clear()
    {
        ((ICollection<SortableItem>)Items).Clear();
    }

    public bool Contains(SortableItem item)
    {
        return ((ICollection<SortableItem>)Items).Contains(item);
    }

    public void CopyTo(SortableItem[] array, int arrayIndex)
    {
        ((ICollection<SortableItem>)Items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<SortableItem> GetEnumerator()
    {
        return ((IEnumerable<SortableItem>)Items).GetEnumerator();
    }

    public int IndexOf(SortableItem item)
    {
        return ((IList<SortableItem>)Items).IndexOf(item);
    }

    public void Insert(int index, SortableItem item)
    {
        ((IList<SortableItem>)Items).Insert(index, item);
    }

    public bool Remove(SortableItem item)
    {
        return ((ICollection<SortableItem>)Items).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<SortableItem>)Items).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
    #endregion
}