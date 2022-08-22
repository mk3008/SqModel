using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public interface IWhenValueSettable<T>
{
    T SetWhenValueClause(ValueClause value);
}

public static class IWhenValueSettableExtension
{
    public static T When<T>(this IWhenValueSettable<T> source, TableClause table, string column)
    => source.SetWhenValueClause(ValueBuilder.ToValue(table, column));

    public static T When<T>(this IWhenValueSettable<T> source, string table, string column)
        => source.SetWhenValueClause(ValueBuilder.ToValue(table, column));

    public static T When<T>(this IWhenValueSettable<T> source, string value)
        => source.SetWhenValueClause(ValueBuilder.ToValue(value));

    public static T When<T>(this IWhenValueSettable<T> source, ValueClause value)
        => source.SetWhenValueClause(value);
}

