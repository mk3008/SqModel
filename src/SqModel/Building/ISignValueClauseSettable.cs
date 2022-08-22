using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public interface ISignValueClauseSettable<T>
{
    T SetSignValueClause(string sign, ValueClause value);
}

public static class ISignValueClauseSettableExtension
{
    public static T Equal<T>(this ISignValueClauseSettable<T> source, TableClause table, string column)
        => source.SetSignValueClause("=", ValueBuilder.ToValue(table, column));

    public static T Equal<T>(this ISignValueClauseSettable<T> source, string table, string column)
        => source.SetSignValueClause("=", ValueBuilder.ToValue(table, column));

    public static T Equal<T>(this ISignValueClauseSettable<T> source, string value)
        => source.SetSignValueClause("=", ValueBuilder.ToValue(value));

    public static T NotEqual<T>(this ISignValueClauseSettable<T> source, TableClause table, string column)
        => source.SetSignValueClause("<>", ValueBuilder.ToValue(table, column));

    public static T NotEqual<T>(this ISignValueClauseSettable<T> source, string table, string column)
        => source.SetSignValueClause("<>", ValueBuilder.ToValue(table, column));

    public static T NotEqual<T>(this ISignValueClauseSettable<T> source, string value)
        => source.SetSignValueClause("<>", ValueBuilder.ToValue(value));

    public static T IsNull<T>(this ISignValueClauseSettable<T> source)
        => source.SetSignValueClause("is", ValueBuilder.GetNullValue());

    public static T IsNotNull<T>(this ISignValueClauseSettable<T> source)
        => source.SetSignValueClause("is", ValueBuilder.GetNotNullValue());
}
