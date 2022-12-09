using Cysharp.Text;
using SqModel.Core.Extensions;
using System.Collections;

namespace SqModel.Core.Clauses;

public class WithClause : IList<CommonTable>, IQueryCommand, IQueryParameter
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

    public string GetCommandText()
    {
        if (!CommonTables.Any()) return string.Empty;

        var sb = ZString.CreateStringBuilder();
        sb.Append("with");
        if (HasRecursiveKeyword) sb.Append(" recursive");
        sb.Append("\r\n");

        var isFisrt = true;
        foreach (var item in CommonTables)
        {
            if (isFisrt)
            {
                isFisrt = false;
            }
            else
            {
                sb.Append(",\r\n");
            }
            sb.Append(item.GetCommandText());
        }
        return sb.ToString();
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (!CommonTables.Any()) return EmptyParameters.Get();
        return CommonTables.Select(x => x.GetParameters()).Merge();
    }

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
}