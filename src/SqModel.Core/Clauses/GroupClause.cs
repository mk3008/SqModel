using SqModel.Core.Extensions;
using System.Collections;

namespace SqModel.Core.Clauses;

public class GroupClause : IList<ValueBase>, IQueryCommand
{
    public GroupClause(IList<ValueBase> items)
    {
        Items = new();
        Items.AddRange(items);
    }

    private List<ValueBase> Items { get; init; }

    public string GetCommandText()
    {
        if (!Items.Any()) throw new IndexOutOfRangeException(nameof(Items));

        /*
         * group by
         *     col1 as c1,
         *     col2 as c2
         */
        return "group by".Join($"\r\n", Items.Select(x => x.GetCommandText().InsertIndent()), $",\r\n");
    }

    public ValueBase this[int index] { get => ((IList<ValueBase>)Items)[index]; set => ((IList<ValueBase>)Items)[index] = value; }

    public int Count => ((ICollection<ValueBase>)Items).Count;

    public bool IsReadOnly => ((ICollection<ValueBase>)Items).IsReadOnly;

    public void Add(ValueBase item)
    {
        ((ICollection<ValueBase>)Items).Add(item);
    }

    public void Clear()
    {
        ((ICollection<ValueBase>)Items).Clear();
    }

    public bool Contains(ValueBase item)
    {
        return ((ICollection<ValueBase>)Items).Contains(item);
    }

    public void CopyTo(ValueBase[] array, int arrayIndex)
    {
        ((ICollection<ValueBase>)Items).CopyTo(array, arrayIndex);
    }

    public IEnumerator<ValueBase> GetEnumerator()
    {
        return ((IEnumerable<ValueBase>)Items).GetEnumerator();
    }

    public int IndexOf(ValueBase item)
    {
        return ((IList<ValueBase>)Items).IndexOf(item);
    }

    public void Insert(int index, ValueBase item)
    {
        ((IList<ValueBase>)Items).Insert(index, item);
    }

    public bool Remove(ValueBase item)
    {
        return ((ICollection<ValueBase>)Items).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<ValueBase>)Items).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)Items).GetEnumerator();
    }
}