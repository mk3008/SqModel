using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public interface IReturnValueClauseSettable
{
    ValueClause SetReturnValueClause(ValueClause value);
}

public static class IReturnValueClauseSettableExtension
{
    public static ValueClause Then(this IReturnValueClauseSettable source, TableClause table, string column)
        => source.SetReturnValueClause(ValueBuilder.ToValue(table, column));

    public static ValueClause Then(this IReturnValueClauseSettable source, string table, string column)
        => source.SetReturnValueClause(ValueBuilder.ToValue(table, column));

    public static ValueClause Then(this IReturnValueClauseSettable source, string value)
        => source.SetReturnValueClause(ValueBuilder.ToValue(value));
}
