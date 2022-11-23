using SqModel.Core.Clauses;

namespace SqModel.Core;

public class SelectQuery //: ITableQuery
{
    public WithClause? WithClause { get; set; }

    public WithClause With()
    {
        WithClause ??= new WithClause();
        return WithClause;
    }

    public SelectClause? SelectClause { get; set; }

    public SelectClause Select()
    {
        SelectClause ??= new SelectClause();
        return SelectClause;
    }

    public FromClause? FromClause { get; set; }

    public FromClause From(SelectableTable table)
    {
        FromClause ??= new FromClause(table);
        return FromClause;
    }

    public WhereClause? WhereClause { get; set; }

    public WhereClause Where(IValue condition)
    {
        WhereClause ??= new WhereClause(condition);
        return WhereClause;
    }

    public ValueListClause? GroupClause { get; set; }

    public ValueListClause GroupBy()
    {
        GroupClause ??= new ValueListClause("group by");
        return GroupClause;
    }

    public ValueListClause? HavingClause { get; set; }

    public ValueListClause Having()
    {
        HavingClause ??= new ValueListClause("having");
        return HavingClause;
    }

    public ValueListClause? OrderClause { get; set; }

    public ValueListClause OrderBy()
    {
        OrderClause ??= new ValueListClause("order by");
        return OrderClause;
    }

    //union

}
