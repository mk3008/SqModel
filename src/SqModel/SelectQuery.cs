﻿using SqModel;
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

    public WithClause WithClause { get; set; } = new();

    public WithClause With => WithClause;

    public IEnumerable<CommonTable> GetCommonTableClauses()
    {
        foreach (var item in FromClause.GetCommonTableClauses()) yield return item;
        foreach (var item in With.GetCommonTableClauses()) yield return item;
    }

    public WithClause GetAllWith()
    {
        var w = new WithClause();
        GetCommonTableClauses().ToList().ForEach(x => w.Collection.Add(x));
        return w;
    }

    public WhereClause WhereClause { get; set; } = new();

    public NamelessItems GroupClause { get; set; } = new("group by");

    public NamelessItems GroupBy => GroupClause;

    public NamelessItems OrderClause { get; set; } = new("order by");

    public NamelessItems OrderBy => OrderClause;

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
        var groupQ = GroupClause.ToQuery();//ex. gtoup by a.id
        var orderQ = OrderClause.ToQuery();//ex. order by a.id

        //command text
        var sb = new StringBuilder();
        //parameter
        var prms = new Dictionary<string, object>();

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
        append(orderQ);

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