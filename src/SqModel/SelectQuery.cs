using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class SelectQuery
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

    public WhereClause WhereClause = new ();

    public Query ToQuery()
    {
        var withQ = GetAllWith().ToQuery();
        var selectQ = SelectClause.ToQuery(); //ex. select column_a, column_b
        var fromQ = FromClause.ToQuery(); //ex. from table_a as a inner join table_b as b on a.id = b.id
        var whereQ = WhereClause.ToQuery();//ex. where a.id = 1

        //parameter
        var prms = new Dictionary<string, object>();
        prms.Merge(fromQ.Parameters);
        prms.Merge(selectQ.Parameters);
        prms.Merge(withQ.Parameters);
        prms.Merge(whereQ.Parameters);

        //command text
        var sb = new StringBuilder();
        if (withQ.CommandText != String.Empty)
        {
            sb.Append(withQ.CommandText);
            sb.Append("\r\n");
        }

        sb.Append($"{selectQ.CommandText}");
        sb.Append($"\r\n{fromQ.CommandText}");
        if (whereQ.CommandText != String.Empty) sb.Append($"\r\n{whereQ.CommandText}");

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