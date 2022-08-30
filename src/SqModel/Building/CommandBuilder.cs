using SqModel.Clause;
using SqModel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public static class CommandBuilder
{
    public static ICommand Create(TableClause table, string column)
        => new ColumnCommand() { Table = table.AliasName, Column = column };

    public static ICommand Create(string table, string column)
        => new ColumnCommand() { Table = table, Column = column };

    public static ICommand Create(object commandtext)
    {
        var val = commandtext?.ToString();
        if (val == null) throw new InvalidProgramException();
        var c = new CommandValue() { CommandText = val };
        return c;
    }

    public static ICommand GetNullValue()
        => new CommandValue() { CommandText = "null" };

    public static ICommand GetNotNullValue()
        => new CommandValue() { CommandText = "not null" };
}