using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

internal static class ValueBuilder
{
    public static IValueClause Create(TableClause table, string column)
        => new ColumnValue() { Table = table.AliasName, Column = column };

    public static IValueClause Create(string table, string column)
        => new ColumnValue() { Table = table, Column = column };

    public static IValueClause Create(object commandtext)
    {
        var val = commandtext?.ToString();
        if (val == null) throw new InvalidProgramException();
        var c = new CommandValue() { CommandText = val };
        return c;
    }

    public static IValueClause GetNullValue()
        => new CommandValue() { CommandText = "null" };

    public static IValueClause GetNotNullValue()
        => new CommandValue() { CommandText = "not null" };

    public static IValueClause GetTrue()
        => new CommandValue() { CommandText = "true" };
    public static IValueClause GetFalse()
        => new CommandValue() { CommandText = "false" };
}