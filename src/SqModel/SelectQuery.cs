using SqModel;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public partial class SelectQuery
{
    public TableClause FromClause { get; set; } = new();

    public SelectClause SelectClause { get; set; } = new();

    public SelectClause Select => SelectClause;

    public WithClause With { get; set; } = new();

    public IEnumerable<CommonTable> GetCommonTableClauses()
    {
        foreach (var item in FromClause.GetCommonTableClauses()) yield return item;
        foreach (var item in With.GetCommonTableClauses()) yield return item;
    }

    public WithClause GetAllWith()
    {
        var w = new WithClause();
        GetCommonTableClauses().ToList().ForEach(x => w.CommonTableAliases.Add(x));
        return w;
    }

    public WhereClause WhereClause = new();

    public ConditionGroup Where => WhereClause.ConditionGroup;

    public bool IsOneLineFormat { get; set; } = false;

    public bool IsincludeCte { get; set; } = true;

    public virtual Query ToQuery()
    {
        var splitter = IsOneLineFormat ? " " : "\r\n";
        WhereClause.IsOneLineFormat = IsOneLineFormat;

        var withQ = (IsincludeCte) ? GetAllWith().ToQuery() : null; //ex. with a as (...)
        var selectQ = SelectClause.ToQuery(); //ex. select column_a, column_b
        var fromQ = FromClause.ToQuery(); //ex. from table_a as a inner join table_b as b on a.id = b.id
        var whereQ = WhereClause.ToQuery();//ex. where a.id = 1

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(fromQ.Parameters);
        prms.Merge(selectQ.Parameters);
        if (withQ != null) prms.Merge(withQ.Parameters);
        prms.Merge(whereQ.Parameters);

        //command text
        var sb = new StringBuilder();
        if (withQ != null && withQ.IsNotEmpty())
        {
            sb.Append(withQ.CommandText);
            sb.Append(splitter);
        }

        sb.Append($"{selectQ.CommandText}");
        sb.Append($"{splitter}{fromQ.CommandText}");
        if (whereQ.IsNotEmpty()) sb.Append($"{splitter}{whereQ.CommandText}");

        return new Query() { CommandText = sb.ToString(), Parameters = prms };
    }

    public SelectQuery PushToCommonTable(string alias)
    {
        var q = new SelectQuery();
        q.With.Add(this, alias);
        return q;
    }

    public CommonTable SearchCommonTable(string alias)
        => GetCommonTableClauses().Where(x => x.Name == alias).First();
}

public static class SelectQueryExtension
{
    public static Query ToCreateTableQuery(this SelectQuery source, string tablename, bool istemporary = true)
    {
        var q = source.ToQuery();
        var tmp = (istemporary) ? "temporary " : "";
        q.CommandText = $"create {tmp}table {tablename}\r\nas\r\n{q.CommandText}";
        return q;
    }

    public static SelectQuery Distinct(this SelectQuery source, bool isdistinct = true)
    {
        source.SelectClause.IsDistinct = isdistinct;
        return source;
    }

    public static Query ToInsertQuery(this SelectQuery source, string tablename)
    {
        var q = source.ToQuery();
        var cols = source.SelectClause.GetColumnNames();
        var coltext = $"({cols.ToString(", ")})";

        q.CommandText = $"insert into {tablename}{coltext}\r\n{q.CommandText}";
        return q;
    }
}