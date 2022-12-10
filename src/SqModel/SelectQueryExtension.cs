using SqModel.Extension;

namespace SqModel;

public static class SelectQueryExtension
{
    public static SelectQuery PushToCommonTable(this SelectQuery source, string alias)
    {
        var q = new SelectQuery();
        q.With.Add(source, alias);
        return q;
    }

    [Obsolete("GetCommonTable")]
    public static CommonTable SearchCommonTable(this SelectQuery source, string alias)
        => source.GetCommonTables().Where(x => x.Name == alias).First();

    [Obsolete("GetCommonTables")]
    public static IEnumerable<CommonTable> GetCommonTableClauses(this SelectQuery source)
        => source.GetCommonTables();

    public static bool CommonTableContains(this SelectQuery source, string alias)
        => source.GetCommonTables().Where(x => x.Name == alias).Any();

    public static IEnumerable<CommonTable> GetCommonTables(this SelectQuery source)
    {
        foreach (var item in source.FromClause.GetCommonTableClauses()) yield return item;
        foreach (var item in source.With.GetCommonTableClauses()) yield return item;
    }

    public static CommonTable? GetCommonTableOrDefault(this SelectQuery source, string alias)
        => source.GetCommonTables().Where(x => x.Name == alias).FirstOrDefault();

    public static CommonTable GetCommonTable(this SelectQuery source, string alias)
        => source.GetCommonTables().Where(x => x.Name == alias).First();

    public static List<SelectItem> GetSelectItems(this SelectQuery source)
        => source.Select.Collection;

    public static bool ColumnContains(this SelectQuery source, string name)
        => source.Select.Collection.Where(x => x.Name == name).Any();

    public static SelectItem GetSelectItemByName(this SelectQuery source, string name)
        => source.Select.Collection.Where(x => x.Name == name).First();

    public static SelectItem? GetSelectItemByNameOrDefault(this SelectQuery source, string name)
        => source.Select.Collection.Where(x => x.Name == name).FirstOrDefault();

    public static bool TableAliasContains(this SelectQuery source, string alias)
        => source.FromClause.GetTableClauses().Where(x => x.AliasName == alias).Any();

    public static List<TableClause> GetTableClauses(this SelectQuery source)
        => source.FromClause.GetTableClauses();

    public static List<TableClause> GetTableClausesByTable(this SelectQuery source, string table)
        => source.FromClause.GetTableClauses().Where(x => x.TableName == table).ToList();

    public static TableClause GetTableClauseByName(this SelectQuery source, string alias)
        => source.FromClause.GetTableClauses().Where(x => x.AliasName == alias).First();

    public static TableClause? GetTableClauseByNameOrDefault(this SelectQuery source, string alias)
        => source.FromClause.GetTableClauses().Where(x => x.AliasName == alias).FirstOrDefault();

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

    public static Query ToInsertQuery(this SelectQuery source, string tablename, List<string> keys)
    {
        if (source.FromClause.TableName == String.Empty)
        {
            return ToInsertFromValue(source, tablename, keys);
        }
        return ToInsertFromQuery(source, tablename, keys);
    }

    private static Query ToInsertFromQuery(this SelectQuery source, string tablename, List<string> keys)
    {
        var filter = (string s) => keys.Contains(s);

        var sq = new SelectQuery();
        var d = sq.From(source).As("d");
        source.Select.Collection.Where(x => filter(x.Name)).ToList().ForEach(x => source.Select.Collection.Remove(x));

        source.Select.GetColumnNames().ForEach(x =>
        {
            sq.Select.Add().Column(d, x);
        });

        var q = sq.ToQuery();
        var coltext = $"({sq.Select.GetColumnNames().ToString(", ")})";

        q.CommandText = $"insert into {tablename}{coltext}\r\n{q.CommandText}";
        return q;
    }

    private static Query ToInsertFromValue(this SelectQuery source, string tablename, List<string> keys)
    {
        var filter = (string s) => keys.Contains(s);

        source.Select.Collection.Where(x => filter(x.Name)).ToList().ForEach(x => source.Select.Collection.Remove(x));

        var q = source.ToQuery();
        var coltext = $"({source.Select.GetColumnNames().ToString(", ")})";

        q.CommandText = $"insert into {tablename}{coltext}\r\n{q.CommandText}";
        return q;
    }

    public static Query ToUpdateQuery(this SelectQuery source, string tablename, List<string> keys)
    {
        if (source.FromClause.TableName == String.Empty)
        {
            return ToUpdateFromValue(source, tablename, keys);
        }
        return ToUpdateFromQuery(source, tablename, keys);
    }

    private static Query ToUpdateFromQuery(this SelectQuery source, string tablename, List<string> keys)
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

    public static Query ToUpdateFromValue(this SelectQuery source, string tablename, List<string> keys)
    {
        var filter = (string s) => keys.Contains(s);
        var alias = "t";

        var q = new Query();
        q.Parameters = source.ToQuery().Parameters;

        var values = source.Select.Collection.Where(x => !filter(x.Name)).ToList();
        var ids = source.Select.Collection.Where(x => filter(x.Name)).ToList();

        //set
        var lst = new List<string>();
        values.ForEach(x =>
        {
            if (x.Command == null) throw new Exception();
            lst.Add($"{x.Name} = {x.Command.ToQuery().CommandText}");
        });

        var wq = new SelectQuery();
        var t = wq.From("tablename").As(alias);
        ids.ForEach(x =>
        {
            if (x.Command == null) throw new Exception();
            wq.Where.Add().Column(t, x.Name).Equal(x.Command.ToQuery().CommandText);
        });

        q.CommandText = $@"update {tablename} as {alias}
set
    {lst.ToString("\r\n    , ")}
{wq.WhereClause.ToQuery().CommandText}";

        return q;
    }
}