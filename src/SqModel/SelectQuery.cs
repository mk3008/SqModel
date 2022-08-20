﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public partial class SelectQuery
{
    public TableClause FromClause { get; set; } = new();

    public SelectClause SelectClause { get; set; } = new();

    public WithClause With { get; set; } = new();

    public IEnumerable<CommonTableClause> GetCommonTableClauses()
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

    public void Distinct(bool isdistinct = true )
        => SelectClause.IsDistinct = isdistinct;

    public WhereClause WhereClause = new();

    public OperatorContainer Where() => WhereClause.Container.Where();

    public Query ToQuery() => ToQueryCore(true);

    public Query ToSubQuery() => ToQueryCore(false);

    public Query ToInlineQuery() => ToQueryCore(false, " ");

    private Query ToQueryCore(bool includeCte, string splitter = "\r\n")
    {
        var withQ = (includeCte) ? GetAllWith().ToQuery() : null; //ex. with a as (...)
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

    public CommonTableClause SearchCommonTable(string alias)
        => GetCommonTableClauses().Where(x => x.AliasName == alias).First();

}