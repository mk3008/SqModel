using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Building;

public interface IReturnValueSettable
{
    ValueClause SetReturnValue(ValueClause value);
}

public static class IReturnValueClauseSettableExtension
{
    public static ValueClause Then(this IReturnValueSettable source, TableClause table, string column)
        => source.SetReturnValue(ValueBuilder.ToValue(table, column));

    public static ValueClause Then(this IReturnValueSettable source, string table, string column)
        => source.SetReturnValue(ValueBuilder.ToValue(table, column));

    public static ValueClause Then(this IReturnValueSettable source, string value)
        => source.SetReturnValue(ValueBuilder.ToValue(value));
}
