using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQuerySelectExtension
{
    public static IValueContainer Select(this SelectQuery source, TableClause table, string column)
        => source.Select.Add().Column(table, column);

    public static IValueContainer Select(this SelectQuery source, string table, string column)
        => source.Select.Add().Column(table, column);

    public static IValueContainer Select(this SelectQuery source, object value)
        => source.Select.Add().Value(value);

    public static void SelectAll(this SelectQuery source, TableClause table)
    {
        if (table.Actual == null)
        {
            source.Select.Add().All(table);
            return;
        }

        var names = source.GetSelectItems().Select(x => x.Name).ToList();
        table.Actual.GetSelectItems().Where(x => !names.Contains(x.Name)).ToList().ForEach(x =>
        {
            source.Select.Add().Column(table, x.Name);
        });
    }

    public static void SelectAll(this SelectQuery source, string table)
        => source.Select.Add().All(table);

    public static void SelectAll(this SelectQuery source)
        => source.Select.Add().All();

    public static void SelectCount(this SelectQuery source)
        => source.Select.Add().Value("count(*)");

    public static void SelectAll(this SelectQuery source, CommonTable table, string? tableAlias = null)
    {
        var tname = tableAlias ?? table.Name;
        var cols = table.Query.GetSelectItems().Select(x => x.Name).ToList();
        cols.ForEach(x => source.Select.Add().Column(tname, x));
    }
}