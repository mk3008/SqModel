using Cysharp.Text;
using SqModel.Core.Extensions;
using System.Collections;

namespace SqModel.Core.Clauses;

public class SelectClause : IList<SelectableItem>, IQueryCommand
{
    public SelectClause()
    {
    }

    public SelectClause(List<SelectableItem> collection)
    {
        Items.AddRange(collection);
    }

    public bool HasDistinctKeyword { get; set; } = false;

    public ValueBase? Top { get; set; }

    private List<SelectableItem> Items { get; init; } = new();

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        if (HasDistinctKeyword && Top != null)
        {
            yield return (tp, "select distinct top " + Top.GetTokens().ToString(" "), BlockType.Start, true);
        }
        else if (HasDistinctKeyword)
        {
            yield return (tp, "select distinct", BlockType.Start, true);
        }
        else if (Top != null)
        {
            yield return (tp, "select top " + Top.GetTokens().ToString(" "), BlockType.Start, true);
        }
        else
        {
            yield return (tp, "select", BlockType.Start, true);
        }

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

    #region implements IList<SelectableItem>
    public SelectableItem this[int index] { get => ((IList<SelectableItem>)Items)[index]; set => ((IList<SelectableItem>)Items)[index] = value; }

    public int Count => ((ICollection<SelectableItem>)Items).Count;

    public bool IsReadOnly => ((ICollection<SelectableItem>)Items).IsReadOnly;

    public void Add(SelectableItem item)
    {
        ((ICollection<SelectableItem>)Items).Add(item);
    }

    public void Clear()
    {
        ((ICollection<SelectableItem>)Items).Clear();
    }

    public bool Contains(SelectableItem item)
    {
        return ((ICollection<SelectableItem>)Items).Contains(item);
    }

    public void CopyTo(SelectableItem[] array, int arrayIndex)
    {
        ((ICollection<SelectableItem>)Items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<SelectableItem> GetEnumerator()
    {
        return ((IEnumerable<SelectableItem>)Items).GetEnumerator();
    }

    public int IndexOf(SelectableItem item)
    {
        return ((IList<SelectableItem>)Items).IndexOf(item);
    }

    public void Insert(int index, SelectableItem item)
    {
        ((IList<SelectableItem>)Items).Insert(index, item);
    }

    public bool Remove(SelectableItem item)
    {
        return ((ICollection<SelectableItem>)Items).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<SelectableItem>)Items).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
    #endregion
}