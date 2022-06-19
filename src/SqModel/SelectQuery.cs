using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class SelectQuery
{
    #region "From"
    public TableClause FromClause { get; set; } = new();

    public TableClause From(string tableName)
    {
        FromClause = new TableClause() { TableName = tableName, AliasName = tableName };
        return FromClause;
    }

    public TableClause From(string tableName, string aliasName)
    {
        FromClause = new TableClause() { TableName = tableName, AliasName = aliasName };
        return FromClause;
    }

    public TableClause From(SelectQuery subquery, string alias)
    {
        FromClause = new TableClause() { SubSelectClause = subquery, AliasName = alias };
        return FromClause;
    }
    #endregion

    #region "Select"
    public SelectClause SelectClause { get; set; } = new();

    public ColumnClause Select(TableClause table, string columnName, string? aliasName = null)
    {
        var c = new ColumnClause() { TableName = table.AliasName, ColumnName = columnName, AliasName = aliasName ?? columnName };
        SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public ColumnClause Select(string commandText, string aliasName)
    {
        var c = new ColumnClause() { CommandText = commandText, AliasName = aliasName };
        SelectClause.ColumnClauses.Add(c);
        return c;
    }
    #endregion

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

    //public List<Condition> Conditions = new();


    //private SelectClause GetColumnClause()
    //{
    //    if (SelectClause.GetColumnNames().Any()) return SelectClause;

    //    var cc = new SelectClause();
    //    cc.Add(FromClause, "*");
    //    JoinTableRelationClause.TableRelationClauses.Select(x => cc.Add(x.DestinationTable, "*"));

    //    return cc;
    //}

    public Query ToQuery()
    {
        var withQ = GetAllWith().ToQuery();
        var selectQ = SelectClause.ToQuery(); //ex. select column_a, column_b
        var fromQ = FromClause.ToQuery(); //ex. from table_a as a inner join table_b as b on a.id = b.id

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(fromQ.Parameters);
        prms.Merge(selectQ.Parameters);
        prms.Merge(withQ.Parameters);

        //command text
        var sb = new StringBuilder();
        if (withQ.CommandText != String.Empty)
        {
            sb.Append(withQ.CommandText);
            sb.Append("\r\n");
        }

        sb.Append($"{selectQ.CommandText}");
        sb.Append("\r\n");
        sb.Append($"{fromQ.CommandText}");

        //if (!string.IsNullOrEmpty(joinQ.CommandText))
        //{
        //    sb.Append("\r\n");
        //    sb.Append(joinQ.CommandText);
        //}

        return new Query() { CommandText = sb.ToString(), Parameters = prms };
    }

    public Query ToSubQuery()
    {
        var selectQ = SelectClause.ToQuery(); //ex. select column_a, column_b
        var fromQ = FromClause.ToQuery(); //ex. from table_a as a inner join table_b as b on a.id = b.id

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(fromQ.Parameters);
        prms.Merge(selectQ.Parameters);

        //command text
        var sb = new StringBuilder();

        var s = selectQ.CommandText;
        var f = fromQ.CommandText;
        if (s == $"select {FromClause.TableName}.*" && f == $"from {FromClause.TableName}")
        {
            sb.Append(FromClause.TableName);
        }
        else
        {
            sb.Append($"(\r\n{s.Indent()}\r\n{f.Indent()}\r\n)");
        }
        return new Query() { CommandText = sb.ToString(), Parameters = prms };
    }

    public Query ToCommonQuery()
    {
        var selectQ = SelectClause.ToQuery(); //ex. select column_a, column_b
        var fromQ = FromClause.ToQuery(); //ex. from table_a as a inner join table_b as b on a.id = b.id

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(fromQ.Parameters);
        prms.Merge(selectQ.Parameters);

        //command text
        var sb = new StringBuilder();

        var s = selectQ.CommandText;
        var f = fromQ.CommandText;
        sb.Append($"(\r\n{s.Indent()}\r\n{f.Indent()}\r\n)");

        return new Query() { CommandText = sb.ToString(), Parameters = prms };
    }
}