using SqModel.Core.Extensions;
using SqModel.Core.Values;
using System.Collections;

namespace SqModel.Core.Clauses;

public class SelectClause : IList<SelectableItem>, IQueryable
{
    public SelectClause()
    {
    }

    public SelectClause(List<SelectableItem> collection)
    {
        Items.AddRange(collection);
    }

    private List<SelectableItem> Items { get; init; } = new();

    public string GetCommandText()
    {
        if (!Items.Any()) throw new IndexOutOfRangeException(nameof(Items));

        /*
         * select
         *     col1 as c1,
         *     col2 as c2
         */
        return "select".Join($"\r\n", Items.Select(x => x.GetCommandText().InsertIndent()), $",\r\n");
    }

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

    public IDictionary<string, object?> GetParameters()
    {
        return Items.Select(x => x.GetParameters()).Merge();
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
}