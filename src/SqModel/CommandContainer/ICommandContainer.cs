using SqModel.Building;
using SqModel.Clause;
using SqModel.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public interface ICommandContainer
{
    public ICommand? Command { get; set; }

    string ColumnName { get; set; }

    string Name { get; set; }
}

public static class ICommandContainerExtension
{
    public static ICommandContainer Value(this ICommandContainer source, object commandtext)
        => source.Value(CommandBuilder.Create(commandtext));

    public static ICommandContainer Column(this ICommandContainer source, TableClause table, string column)
        => source.Value(CommandBuilder.Create(table, column), column);

    public static ICommandContainer Column(this ICommandContainer source, string table, string column)
        => source.Value(CommandBuilder.Create(table, column), column);

    public static ICommandContainer Null(this ICommandContainer source)
        => source.Value(CommandBuilder.GetNullValue());

    public static ICommandContainer NotNull(this ICommandContainer source)
        => source.Value(CommandBuilder.GetNotNullValue());

    public static ICommandContainer Value(this ICommandContainer source, ICommand value, string column = "")
    {
        source.Command = value;
        source.ColumnName = column;
        return source;
    }

    public static ICommandContainer As(this ICommandContainer source, string name)
    {
        source.Name = name;
        return source;
    }

    public static ICommandContainer Parameter(this ICommandContainer source, string name, object value)
    {
        if (source.Command == null) throw new InvalidProgramException();
        source.Command.AddParameter(name, value);
        return source;
    }
}

