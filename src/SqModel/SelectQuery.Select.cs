using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQuerySelect
{
    public static ColumnClause Select(this SelectQuery source, TableClause table, string columnName, string? aliasName = null)
    {
        var c = new ColumnClause() { TableName = table.AliasName, ColumnName = columnName, AliasName = aliasName ?? columnName };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }

    public static ColumnClause Select(this SelectQuery source, string commandText, string aliasName)
    {
        var c = new ColumnClause() { CommandText = commandText, AliasName = aliasName };
        source.SelectClause.ColumnClauses.Add(c);
        return c;
    }
}

