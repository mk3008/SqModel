using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryWhere
{

    public static void And(this OperatorContainer source, Action<OperatorContainer> fn)
    {
        var group = new OperatorContainer() { Operator = "and" };
        source.Add(group);
        fn(group);
    }

    public static void Or(this OperatorContainer source, Action<OperatorContainer> fn)
    {
        var group = new OperatorContainer() { Operator = "or" };
        source.Add(group);
        fn(group);
    }

    public static ValueContainer And(this OperatorContainer source, TableClause table, string column)
        => source.Operate("and", ValueBuilder.ToValue(table, column));

    public static ValueContainer And(this OperatorContainer source, string table, string column)
        => source.Operate("and", ValueBuilder.ToValue(table, column));

    public static ValueContainer And(this OperatorContainer source, string value)
        => source.Operate("and", ValueBuilder.ToValue(value));

    public static ValueContainer Or(this OperatorContainer source, TableClause table, string column)
        => source.Operate("or", ValueBuilder.ToValue(table, column));

    public static ValueContainer Or(this OperatorContainer source, string table, string column)
        => source.Operate("or", ValueBuilder.ToValue(table, column));

    public static ValueContainer Or(this OperatorContainer source, string value)
        => source.Operate("or", ValueBuilder.ToValue(value));

    public static ValueContainer Operate(this OperatorContainer source, string @operator, ValueClause value)
    {
        var c = new OperatorContainer() { Operator = @operator };
        source.Add(c);
        c.Condition = new();
        c.Condition.Source = value;
        return c.Condition;
    }

    public static ValueClause Equal(this ValueContainer source, TableClause table, string column)
        => source.Sign("=", ValueBuilder.ToValue(table, column));

    public static ValueClause Equal(this ValueContainer source, string table, string column)
        => source.Sign("=", ValueBuilder.ToValue(table, column));

    public static ValueClause Equal(this ValueContainer source, string value)
        => source.Sign("=", ValueBuilder.ToValue(value));

    public static ValueClause NotEqual(this ValueContainer source, TableClause table, string column)
        => source.Sign("<>", ValueBuilder.ToValue(table, column));

    public static ValueClause NotEqual(this ValueContainer source, string table, string column)
        => source.Sign("<>", ValueBuilder.ToValue(table, column));

    public static ValueClause NotEqual(this ValueContainer source, string value)
        => source.Sign("<>", ValueBuilder.ToValue(value));

    public static ValueClause IsNull(this ValueContainer source)
        => source.Sign("is", ValueBuilder.GetNullValue());

    public static ValueClause IsNotNull(this ValueContainer source)
        => source.Sign("is", ValueBuilder.GetNotNullValue());

    public static ValueClause Sign(this ValueContainer source, string sign, ValueClause value)
    {
        var c = new ValueConjunction() { Sign = sign };
        source.ValueConjunction = c;
        c.Destination = value;
        return value;
    }
}
