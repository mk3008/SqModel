using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryExtension
{
    public static List<SelectItem> GetSelectItems(this SelectQuery source)
        => source.Select.Collection;

    public static List<TableClause> GetTableClauses(this SelectQuery source)
        => source.FromClause.GetTableClauses();

    public static Query ToCreateTableQuery(this SelectQuery source, string tablename, bool istemporary = true)
    {
        var q = source.ToQuery();
        var tmp = (istemporary) ? "temporary " : "";
        q.CommandText = $"create {tmp}table {tablename}\r\nas\r\n{q.CommandText}";
        return q;
    }

    public static SelectQuery Distinct(this SelectQuery source, bool isdistinct = true)
    {
        source.Select.IsDistinct = isdistinct;
        return source;
    }

    public static Query ToInsertQuery(this SelectQuery source, string tablename, List<string>? ignorecols = null)
    {
        var filter = (string x) =>
        {
            if (ignorecols == null) return true;
            return !ignorecols.Contains(x);
        };

        var sq = new SelectQuery();
        var d = sq.From(source).As("d");
        source.Select.GetColumnNames().Where(x => filter(x)).ToList().ForEach(x =>
        {
            sq.Select.Add().Column(d, x);
        });

        var q = sq.ToQuery();
        var coltext = $"({sq.Select.GetColumnNames().ToString(", ")})";

        q.CommandText = $"insert into {tablename}{coltext}\r\n{q.CommandText}";
        return q;
    }

    public static Query ToUpdateQuery(this SelectQuery source, string tablename, List<string> keys)
    {
        var t = "t";
        var cols = source.Select.GetColumnNames();

        //where
        var sq = new SelectQuery();
        var d = sq.From(source).As("d");
        keys.ForEach(x => sq.Where.Add().Column(t, x).Equal(d, x));

        //set
        var lst = new List<string>();
        cols.Where(x => !keys.Contains(x)).ToList().ForEach(x =>
        {
            lst.Add($"{x} = {d.AliasName}.{x}");
        });

        var fromQ = sq.FromClause.ToQuery();
        var whereQ = sq.WhereClause.ToQuery();

        var q = new Query();
        q.CommandText = $@"update {tablename} as {t}
set
    {lst.ToString("\r\n    , ")}
{fromQ.CommandText}
{whereQ.CommandText}";

        q.Parameters = q.Parameters.Merge(fromQ.Parameters).Merge(whereQ.Parameters);

        return q;
    }
}