using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class OperatorContainerExtension
{
    public static void Group(this OperatorContainer source, Action<OperatorContainer> fn)
    {
        var group = new OperatorContainer() { Operator = "and" };
        source.Add(group);
        fn(group);
    }

    public static ValueContainer Value(this OperatorContainer source, TableClause table, string column)
        => source.Value(ValueBuilder.ToValue(table, column));

    public static ValueContainer Value(this OperatorContainer source, string table, string column)
        => source.Value(ValueBuilder.ToValue(table, column));

    public static ValueContainer Value(this OperatorContainer source, string value)
        => source.Value(ValueBuilder.ToValue(value));

    public static ValueContainer Value(this OperatorContainer source, ValueClause value)
    {
        source.Condition = new() { Source = value };
        return source.Condition;
    }

    public static void Exists(this OperatorContainer source, Func<SelectQuery> fn)
    {
        source.Condition ??= new();
        source.Condition.ExistsQuery = fn.Invoke();
    }

    public static void Exists(this OperatorContainer source, SelectQuery existsQuery)
    {
        source.Condition ??= new();
        source.Condition.ExistsQuery = existsQuery;
    }
}
