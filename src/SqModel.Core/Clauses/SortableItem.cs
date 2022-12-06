using Cysharp.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Core.Clauses;

public class SortableItem : IQueryCommand
{
    public SortableItem(IValue query, bool isAscending = true, NullSortType tp = NullSortType.Undefined)
    {
        Value = query;
        IsAscending = isAscending;
        NullSort = tp;
    }

    public IValue Value { get; init; }

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
}