using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQuerySelect
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
}

