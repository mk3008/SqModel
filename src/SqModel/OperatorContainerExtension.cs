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

    public static ValueContainer CaseWhen(this OperatorContainer source, Action<CaseExpression<CaseWhenConditionValuePair>> action)
    {
        var c = new CaseExpression<CaseWhenConditionValuePair>();
        source.Condition ??= new();
        source.Condition.Source ??= new();
        source.Condition.Source.CaseExpression = c;
        action(c);
        return source.Condition;
    }

    public static ValueContainer Case(this OperatorContainer source, TableClause table, string column, Action<CaseExpression<CaseConditionValuePair>> action)
        => source.Case(ValueBuilder.ToValue(table, column), action);

    public static ValueContainer Case(this OperatorContainer source, string table, string column, Action<CaseExpression<CaseConditionValuePair>> action)
        => source.Case(ValueBuilder.ToValue(table, column), action);

    public static ValueContainer Case(this OperatorContainer source, string value, Action<CaseExpression<CaseConditionValuePair>> action)
        => source.Case(ValueBuilder.ToValue(value), action);

    public static ValueContainer Case(this OperatorContainer source, ValueClause value, Action<CaseExpression<CaseConditionValuePair>> action)
    {
        var c = new CaseExpression<CaseConditionValuePair>();
        source.Condition ??= new();
        source.Condition.Source ??= new();
        source.Condition.Source.CaseExpression = c;
        c.Value = value;
        action(c);
        return source.Condition;
    }
}
