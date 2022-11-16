using SqModel;
using SqModel.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public partial class SelectQuery
{
    public TableClause FromClause { get; set; } = new();

    public SelectClause? SelectClause { get; set; }

    public SelectClause Select => GetSelectClause();

    private SelectClause GetSelectClause()
    {
        SelectClause ??= new();
        return SelectClause;
    }

    public ValuesClause? ValuesClause { get; set; } = null;

    public ValuesClause Values => GetValueClause();

    private ValuesClause GetValueClause()
    {
        ValuesClause ??= new();
        return ValuesClause;
    }

    public WithClause WithClause { get; set; } = new();

    public WithClause With => WithClause;

    public WithClause GetAllWith()
    {
        var w = new WithClause();
        this.GetCommonTables().ToList().ForEach(x => w.Collection.Add(x));

        if (UnionClause != null && UnionClause.SelectQuery != null)
        {
            w.Collection.AddRange(UnionClause.SelectQuery.GetAllWith().Collection);
        }

        return w;
    }

    public ConditionClause WhereClause { get; set; } = new("where");

    public ConditionGroup Where => WhereClause.ConditionGroup;

    public NamelessItemClause GroupClause { get; set; } = new("group by");

    public NamelessItemClause GroupBy => GroupClause;

    public ConditionClause HavingClause { get; set; } = new("having");

    public ConditionGroup Having => HavingClause.ConditionGroup;

    public UnionClause? UnionClause { get; set; } = null;

    public NamelessItemClause OrderClause { get; set; } = new("order by");

    public NamelessItemClause OrderBy => OrderClause;

    public bool IsOneLineFormat { get; set; } = false;

    public bool IsIncludeCte { get; set; } = true;

    public bool IsIncludeOrder { get; set; } = true;

    public Dictionary<string, object?>? Parameters = null;

    public Query ToQuery()
    {
        var splitter = IsOneLineFormat ? " " : "\r\n";

        if (ValuesClause == null && SelectClause == null) throw new InvalidProgramException();
        if (ValuesClause != null && SelectClause != null) throw new InvalidProgramException();

        if (ValuesClause != null) return ValuesClause.ToQuery();

        if (SelectClause == null) throw new InvalidProgramException("Select clause is null");
        SelectClause.IsOneLineFormat = IsOneLineFormat;
        WhereClause.IsOneLineFormat = IsOneLineFormat;
        OrderClause.IsOneLineFormat = IsOneLineFormat;
        GroupClause.IsOneLineFormat = IsOneLineFormat;

        var withQ = (IsIncludeCte) ? GetAllWith().ToQuery() : null; //ex. with a as (...)
        var selectQ = SelectClause.ToQuery(); //ex. select column_a, column_b
        var fromQ = FromClause.ToQuery(); //ex. from table_a as a inner join table_b as b on a.id = b.id
        var whereQ = WhereClause.ToQuery();//ex. where a.id = 1
        var groupQ = GroupClause.ToQuery();//ex. group by a.id
        var havingQ = HavingClause.ToQuery();//ex. having sum(a.value) = 10
        var unionQ = UnionClause?.ToQuery();//ex. union select...
        var orderQ = (IsIncludeOrder) ? OrderClause.ToQuery() : null; //ex. order by a.id

        //command text
        var sb = new StringBuilder();
        //parameter
        var prms = new Dictionary<string, object?>();

        var append = (Query? query) =>
        {
            if (query == null) return;
            prms.Merge(query.Parameters);
            if (query.IsNotEmpty())
            {
                if (sb.Length != 0) sb.Append(splitter);
                sb.Append($"{query.CommandText}");
            }
        };

        append(withQ);
        append(selectQ);
        append(fromQ);
        append(whereQ);
        append(groupQ);
        append(havingQ);
        append(unionQ);
        append(orderQ);

        prms.Merge(Parameters);

        return new Query() { CommandText = sb.ToString(), Parameters = prms };
    }


}