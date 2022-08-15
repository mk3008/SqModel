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




    //    public static ValueConjunction Equal(this OperatorContainer source)
    //        => new ValueConjunction() { Sign = "=" };

    //    public static ValueConjunction NotEqual(this OperatorContainer source)
    //        => new ValueConjunction() { Sign = "<>" };

    //    public static ValueConjunction Sign(this OperatorContainer source, string sign)
    //        => new ValueConjunction() { Sign = sign };

    //    public static void IsNull(this OperatorContainer source, string table, string column)
    //    {
    //        var s = new ValueConjunction() { Sign = "is" };
    //        s.Value.Source = ToValue(table, column);
    //        s.Value.Destination = GetNullValue();
    //    }

    //    public static void IsNotNull(this OperatorContainer source, string table, string column)
    //    {
    //        var s = new ValueConjunction() { Sign = "is" };
    //        s.Value.Source = ToValue(table, column);
    //        s.Value.Destination = GetNotNullValue();
    //    }

    //    public static ValueContainer WhereAnd(this SelectQuery source, TableClause table, string column, string parameterName, object parameterValue)
    //    => source.Where("and", table, column, "=", parameterName, parameterValue);

    //    public static ValueContainer WhereOr(this SelectQuery source, TableClause table, string column, string parameterName, object parameterValue)
    //=> source.Where("and", table, column, "=", parameterName, parameterValue);

    //    private static ValueContainer Where(this SelectQuery source, string operatorToekn, TableClause table, string column, string sign, string parameterName, object parameterValue)
    //    {
    //        var c = new ValueContainer();
    //        c.Operator = operatorToekn;
    //        c.Source = new ValueClause() { TableName = table.AliasName, Value = column };
    //        c.Sign = sign;
    //        c.Destination = new ValueClause() { Value = parameterName };
    //        c.Destination.AddParameter(parameterName, parameterValue);

    //        source.WhereClause.Container.Conditions.Add(c);
    //        return c;
    //    }

    //    public static ValueContainer WhereAnd(this SelectQuery source, string sourcevalue, string sign, string destinationvalue) => source.Where("and", sourcevalue, sign, destinationvalue);

    //    public static ValueContainer WhereOr(this SelectQuery source, string sourcevalue, string sign, string destinationvalue) => source.Where("or", sourcevalue, sign, destinationvalue);

    //    private static ValueContainer Where(this SelectQuery source, string operatorToekn, string sourcevalue, string sign, string destinationvalue)
    //    {
    //        var c = new ValueContainer();
    //        c.Operator = operatorToekn;
    //        c.Source = new ValueClause() { Value = sourcevalue };
    //        c.Sign = sign;
    //        c.Destination = new ValueClause() { Value = destinationvalue };

    //        source.WhereClause.Container.Conditions.Add(c);
    //        return c;
    //    }

    //    public static void WhereAnd(this SelectQuery source, Action<ConditionGroupClause> action) => source.Where("and", action);

    //    public static void WhereOr(this SelectQuery source, Action<ConditionGroupClause> action) => source.Where("or", action);

    //    private static void Where(this SelectQuery source, string logicalOperator, Action<ConditionGroupClause> action)
    //    {
    //        var g = new ConditionGroupClause();
    //        g.Operator = logicalOperator;
    //        source.WhereClause.Container.GroupConditions.Add(g);
    //        action(g);
    //    }

    //public static void IsNull(this WhereClause source, TableClause table, string column)
    //{
    //    var value = new ValueClause() { TableName = table.AliasName, Value = column };
    //    IsNull(source, value);
    //}
    //public static void IsNull(this WhereClause source, string column)
    //{
    //    var value = new ValueClause() { Value = column };
    //    IsNull(source, value);
    //}

    //public static void IsNull(this WhereClause source, ValueClause value)
    //{
    //    var c = new ConditionClause();
    //    c.Operator = "and";
    //    c.Source = value;
    //    c.Sign = "is";
    //    c.Destination = new ValueClause() { Value = "null" };

    //    source.OperatorContainer.Conditions.Add(c);
    //}

    //public static void IsNotNull(this WhereClause source, TableClause table, string column)
    //{
    //    var value = new ValueClause() { TableName = table.AliasName, Value = column };
    //    IsNotNull(source, value);
    //}
    //public static void IsNotNull(this WhereClause source, string column)
    //{
    //    source.Where("and", ToValue(column), "is", GetNotNullValue());
    //}

    //public static void Where(this WhereClause source,string operatorToken, ValueClause sourceValue, string sign, ValueClause destinationValue)
    //{
    //    var c = new ConditionClause();
    //    c.Operator = operatorToken;
    //    c.Source = sourceValue;
    //    c.Sign = sign;
    //    c.Destination = destinationValue;
    //    source.OperatorContainer.Conditions.Add(c);
    //}


    //public class ConditionBuilder
    //{
    //    public List<ConditionClause> Conditions { get; set; } = new();

    //    public ConditionBuilder And(ValueClause sourceValue, string sign, ValueClause destinationValue)
    //    {
    //        var c = new ConditionClause();
    //        c.Operator = "and";
    //        c.Source = sourceValue;
    //        c.Sign = sign;
    //        c.Destination = destinationValue;
    //        Conditions.Add(c);
    //        return this;
    //    }

    //    public ConditionBuilder Or(ValueClause sourceValue, string sign, ValueClause destinationValue)
    //    {
    //        var c = new ConditionClause();
    //        c.Operator = "or";
    //        c.Source = sourceValue;
    //        c.Sign = sign;
    //        c.Destination = destinationValue;
    //        Conditions.Add(c);
    //        return this;
    //    }
    //}
}
