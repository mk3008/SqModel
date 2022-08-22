using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public static class SelectQuerySelectExtension
{
    public static ValueClause Select(this SelectQuery source, TableClause table, string columnName, string? aliasName = null)
    {
        var c = new ValueClause() { TableName = table.AliasName, Value = columnName, AliasName = aliasName ?? columnName };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause Select(this SelectQuery source, string value, string aliasName)
    {
        var c = new ValueClause() { Value = value, AliasName = aliasName };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause Select(this SelectQuery source, string value)
    {
        var c = new ValueClause() { Value = value };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause Select(this SelectQuery source, SelectQuery inlinequery, string aliasName)
    {
        var c = new ValueClause() { InlineQuery = inlinequery, AliasName = aliasName };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause Select(this SelectQuery source, Func<SelectQuery> fn, string aliasName)
        => fn().Select(aliasName);

    public static ValueClause SelectAll(this SelectQuery source, TableClause table)
    {
        var c = new ValueClause() { TableName = table.AliasName, Value = "*" };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause SelectAll(this SelectQuery source, string table)
    {
        var c = new ValueClause() { TableName = table, Value = "*" };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause SelectAll(this SelectQuery source)
    {
        var c = new ValueClause() { Value = "*" };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ValueClause SelectCount(this SelectQuery source, string? alias = null)
    {
        var c = new ValueClause() { Value = "count(*)" };
        if (alias != null) c.AliasName = alias;
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static IEnumerable<string> GetColumns(this SelectQuery source)
        => source.SelectClause.GetColumnNames();
}

