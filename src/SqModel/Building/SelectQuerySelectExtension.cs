using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqModel.Clause;
using SqModel.Command;
using SqModel.CommandContainer;

namespace SqModel.Building;

public static class SelectQuerySelectExtension
{
    public static ICommandContainer Select(this SelectQuery source, TableClause table, string column)
        => source.Select.Add().Column(table, column);

    public static ICommandContainer Select(this SelectQuery source, string table, string column)
        => source.Select.Add().Column(table, column);

    public static ICommandContainer Select(this SelectQuery source, object value)
        => source.Select.Add().Value(value);

    //public static ValueClause Select(this SelectQuery source, SelectQuery inlinequery, string aliasName)
    //{
    //    var c = new ValueClause() { InlineQuery = inlinequery, AliasName = aliasName };
    //    source.SelectClause.ColumnClauses.Add(c);
    //    return c;
    //}

    //public static SelectItem Select(this SelectQuery source, Action<SelectQuery> action)
    //    => source.Select.Add().InlineQuery(action);

    public static void SelectAll(this SelectQuery source, TableClause table)
        => source.Select.Add().All(table);

    public static void SelectAll(this SelectQuery source, string table)
        => source.Select.Add().All(table);

    public static void SelectAll(this SelectQuery source)
        => source.Select.Add().All();

    //public static ValueClause SelectCount(this SelectQuery source, string? alias = null)
    //{
    //    var c = new ValueClause() { Value = "count(*)" };
    //    if (alias != null) c.AliasName = alias;
    //    source.SelectClause.ColumnClauses.Add(c);
    //    return c;
    //}

    //public static IEnumerable<string> GetColumns(this SelectQuery source)
    //    => source.SelectClause.GetColumnNames();
}

