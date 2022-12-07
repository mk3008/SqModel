using Cysharp.Text;

namespace SqModel.Core.Clauses;

public class SortableItem : IQueryCommand, IQueryParameter
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

    public string GetCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        sb.Append(Value.GetCommandText());
        if (!IsAscending) sb.Append(" desc");
        if (NullSort == NullSortType.Undefined) return sb.ToString();
        sb.Append(" " + NullSort.ToSortText());
        return sb.ToString();
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Value.GetParameters();
    }
}