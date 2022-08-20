using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class ValueBuilder
{
    public static ValueClause ToValue(TableClause table, string column)
        => new ValueClause() { TableName = table.AliasName, Value = column };

    public static ValueClause ToValue(string table, string column)
        => new ValueClause() { TableName = table, Value = column };

    public static ValueClause ToValue(string column)
        => new ValueClause() { Value = column };

    public static ValueClause GetNullValue()
        => new ValueClause() { Value = "null" };

    public static ValueClause GetNotNullValue()
        => new ValueClause() { Value = "not null" };
}
