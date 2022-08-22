using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public interface IReturnValueClauseSettable
{
    ValueClause SetReturnValueClause(ValueClause value);

    //public static ValueClause Then(this ConditionValuePair source, ValueClause value)
    //{
    //    source.ReturnValue = value;
    //    return value;
    //}
}

public static class IReturnValueClauseSettableExtension
{
    //public static ConditionValuePair When(this ConditionValuePair source, TableClause table, string column)
    //    => source.When(ValueBuilder.ToValue(table, column));

    //public static ConditionValuePair When(this ConditionValuePair source, string table, string column)
    //    => source.When(ValueBuilder.ToValue(table, column));

    //public static ConditionValuePair When(this ConditionValuePair source, string value)
    //    => source.When(ValueBuilder.ToValue(value));

    //public static ConditionValuePair When(this ConditionValuePair source, ValueClause value)
    //{
    //    source.ConditionValue = value;
    //    return source;
    //}

    public static ValueClause Then(this IReturnValueClauseSettable source, TableClause table, string column)
        => source.SetReturnValueClause(ValueBuilder.ToValue(table, column));

    public static ValueClause Then(this IReturnValueClauseSettable source, string table, string column)
        => source.SetReturnValueClause(ValueBuilder.ToValue(table, column));

    public static ValueClause Then(this IReturnValueClauseSettable source, string value)
        => source.SetReturnValueClause(ValueBuilder.ToValue(value));
}
