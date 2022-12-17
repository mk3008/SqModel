using Cysharp.Text;
using SqModel.Core.Extensions;
using System.Collections;

namespace SqModel.Core.Clauses;

public class WithClause : IList<CommonTable>, IQueryCommand
{
    public WithClause()
    {
    }

    public WithClause(List<CommonTable> commons)
    {
        CommonTables.AddRange(commons);
    }

    private List<CommonTable> CommonTables { get; set; } = new();

    public bool HasRecursiveKeyword { get; set; } = false;

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        if (HasRecursiveKeyword)
        {
            yield return (tp, "with recursive", BlockType.Start, true);
        }
        else
        {
            yield return (tp, "with", BlockType.Start, true);
        }

        var isFisrt = true;
        foreach (var item in CommonTables)
        {
            if (isFisrt)
            {
                isFisrt = false;
            }
            else
            {
                yield return (tp, ",", BlockType.Split, true);
            }
            foreach (var token in item.GetTokens()) yield return token;
        }
        yield return (tp, string.Empty, BlockType.End, true);
    }

    #region implements IList<CommonTable>
    public CommonTable this[int index] { get => ((IList<CommonTable>)CommonTables)[index]; set => ((IList<CommonTable>)CommonTables)[index] = value; }

    public int Count => ((ICollection<CommonTable>)CommonTables).Count;

    public bool IsReadOnly => ((ICollection<CommonTable>)CommonTables).IsReadOnly;

    public void Add(CommonTable item)
    {
        ((ICollection<CommonTable>)CommonTables).Add(item);
    }

    public void Clear()
    {
        ((ICollection<CommonTable>)CommonTables).Clear();
    }

    public bool Contains(CommonTable item)
    {
        return ((ICollection<CommonTable>)CommonTables).Contains(item);
    }

    public void CopyTo(CommonTable[] array, int arrayIndex)
    {
        ((ICollection<CommonTable>)CommonTables).CopyTo(array, arrayIndex);
    }

    public IEnumerator<CommonTable> GetEnumerator()
    {
        return ((IEnumerable<CommonTable>)CommonTables).GetEnumerator();
    }

    public int IndexOf(CommonTable item)
    {
        return ((IList<CommonTable>)CommonTables).IndexOf(item);
    }

    public void Insert(int index, CommonTable item)
    {
        ((IList<CommonTable>)CommonTables).Insert(index, item);
    }

    public bool Remove(CommonTable item)
    {
        return ((ICollection<CommonTable>)CommonTables).Remove(item);
    }

    public void RemoveAt(int index)
    {
        ((IList<CommonTable>)CommonTables).RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)CommonTables).GetEnumerator();
    }
    #endregion
}