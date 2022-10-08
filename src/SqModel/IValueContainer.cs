using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public interface IValueContainer : IQueryable
{
    public IValueClause? Command { get; set; }

    string ColumnName { set; }

    string Name { set; }
}

public static class IValueContainerExtension
{
    public static IValueContainer Value(this IValueContainer source, object commandtext)
        => source.Value(ValueBuilder.Create(commandtext));

    public static IValueContainer Column(this IValueContainer source, TableClause table, string column)
        => source.Value(ValueBuilder.Create(table, column), column);

    public static IValueContainer Column(this IValueContainer source, string table, string column)
        => source.Value(ValueBuilder.Create(table, column), column);

    public static IValueContainer Null(this IValueContainer source)
        => source.Value(ValueBuilder.GetNullValue());

    public static IValueContainer NotNull(this IValueContainer source)
        => source.Value(ValueBuilder.GetNotNullValue());

    public static IValueContainer Value(this IValueContainer source, IValueClause value, string column = "")
    {
        source.Command = value;
        source.ColumnName = column;
        return source;
    }

    public static IValueContainer As(this IValueContainer source, string name)
    {
        source.Name = name;
        return source;
    }

    [Obsolete("use AddParameter")]
    public static IValueContainer Parameter(this IValueContainer source, string name, object? value)
        => AddParameter(source, name, value);

    public static IValueContainer AddParameter(this IValueContainer source, string name, object? value)
    {
        if (source.Command == null) throw new InvalidProgramException();
        source.Command.AddParameter(name, value);
        return source;
    }
}

