using System;
using System.Collections.Generic;
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
        => source.Select.Add().All(table);

    public static void SelectAll(this SelectQuery source, string table)
        => source.Select.Add().All(table);

    public static void SelectAll(this SelectQuery source)
        => source.Select.Add().All();

    public static void SelectCount(this SelectQuery source)
        => source.Select.Add().Value("count(*)");
}