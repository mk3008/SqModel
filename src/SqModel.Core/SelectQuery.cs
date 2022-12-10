using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core;

public class SelectQuery : QueryBase, IQueryCommandable
{
    //public WithClause With()
    //{
    //    WithClause ??= new WithClause();
    //    return WithClause;
    //}

    public SelectClause? SelectClause { get; set; }

    //public SelectClause Select()
    //{
    //    SelectClause ??= new SelectClause();
    //    return SelectClause;
    //}

    public FromClause? FromClause { get; set; }

    //public FromClause From(SelectableTable table)
    //{
    //    FromClause ??= new FromClause(table);
    //    return FromClause;
    //}

    public WhereClause? WhereClause { get; set; }

    //public WhereClause Where(IValue condition)
    //{
    //    WhereClause ??= new WhereClause(condition);
    //    return WhereClause;
    //}

    public GroupClause? GroupClause { get; set; }

    //public ValueListClause GroupBy()
    //{
    //    GroupClause ??= new ValueListClause("group by");
    //    return GroupClause;
    //}

    public HavingClause? HavingClause { get; set; }

    //public ValueListClause Having()
    //{
    //    HavingClause ??= new ValueListClause("having");
    //    return HavingClause;
    //}


    public override string GetCurrentCommandText()
    {
        if (SelectClause == null) throw new InvalidProgramException();

        var sb = ZString.CreateStringBuilder();
        sb.Append(SelectClause.GetCommandText());
        if (FromClause != null) sb.Append("\r\n" + FromClause.GetCommandText());
        if (WhereClause != null) sb.Append("\r\n" + WhereClause.GetCommandText());
        if (GroupClause != null) sb.Append("\r\n" + GroupClause.GetCommandText());
        if (HavingClause != null) sb.Append("\r\n" + HavingClause.GetCommandText());

        return sb.ToString();
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(SelectClause!.GetParameters());
        prm = prm.Merge(FromClause!.GetParameters());
        prm = prm.Merge(WhereClause!.GetParameters());
        prm = prm.Merge(GroupClause!.GetParameters());
        prm = prm.Merge(HavingClause!.GetParameters());

        return prm;
    }

    //public ValueListClause OrderBy()
    //{
    //    OrderClause ??= new ValueListClause("order by");
    //    return OrderClause;
    //}

    //union

}
