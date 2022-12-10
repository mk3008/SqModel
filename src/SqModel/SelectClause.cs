using SqModel;
using SqModel.Extension;

namespace SqModel;

/// <summary>
/// Select column clause.
/// Define a list of columns to get.
/// </summary>
public class SelectClause
{
    public bool IsDistinct { get; set; } = false;

    public List<SelectItem> Collection { get; set; } = new();

    public bool IsOneLineFormat { get; set; } = true;

    public Query ToQuery()
    {
        var distinct = IsDistinct ? " distinct" : "";

        if (IsOneLineFormat)
        {
            var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery(", ");
            q.CommandText = $"select{distinct} {q.CommandText}";
            return q;
        }
        else
        {
            var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery("\r\n, ").InsertIndent();
            q.CommandText = $"select{distinct}\r\n{q.CommandText}";
            return q;
        }
    }
}

public static class SelectClauseExtension
{
    public static void Distinct(this SelectClause source, bool isdistinct = true)
        => source.IsDistinct = isdistinct;

    public static List<string> GetColumnNames(this SelectClause source)
    {
        var lst = new List<string>();

        source.Collection.ForEach(x => lst.Add(x.Name.IsNotEmpty() ? x.Name : x.ColumnName));

        if (lst.Where(x => x == "*").Any()) return new();
        return lst;
    }

    public static SelectItem Add(this SelectClause source)
    {
        var c = new SelectItem();
        source.Collection.Add(c);
        return c;
    }

    public static void AddRange(this SelectClause source, TableClause table, List<string> columns)
    {
        columns.ForEach(x =>
        {
            var c = new SelectItem();
            c.Column(table, x);
            source.Collection.Add(c);
        });
    }

    public static void AddRangeOrDefault(this SelectClause source, TableClause table, List<string> columns)
    {
        var name = source.GetColumnNames();
        var cols = columns.Where(x => !name.Contains(x)).ToList();
        source.AddRange(table, cols);
    }
}