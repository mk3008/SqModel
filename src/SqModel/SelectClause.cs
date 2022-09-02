using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    public Query ToQuery()
    {
        var q = Collection.Select(x => x.ToQuery()).ToList().ToQuery(", ");
        var distinct = IsDistinct ? "distinct " : "";
        q.CommandText = $"select {distinct}{q.CommandText}";

        return q;
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
}