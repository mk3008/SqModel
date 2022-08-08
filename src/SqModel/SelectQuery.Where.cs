using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public static class SelectQueryWhere
{
    public static ConditionClause WhereAnd(this SelectQuery source, TableClause table, string column, string parameterName, object parameterValue)
    => source.Where("and", table, column, "=", parameterName, parameterValue);

    public static ConditionClause WhereOr(this SelectQuery source, TableClause table, string column, string parameterName, object parameterValue)
=> source.Where("and", table, column, "=", parameterName, parameterValue);

    private static ConditionClause Where(this SelectQuery source, string operatorToekn, TableClause table, string column, string sign, string parameterName, object parameterValue)
    {
        var c = new ConditionClause();
        c.Operator = operatorToekn;
        c.Source = new ValueClause() { TableName = table.AliasName, Value = column };
        c.Sign = sign;
        c.Destination = new ValueClause() { Value = parameterName };
        c.Destination.AddParameter(parameterName, parameterValue);

        source.WhereClause.ConditionClause.Conditions.Add(c);
        return c;
    }

    public static ConditionClause WhereAnd(this SelectQuery source, string sourcevalue, string sign, string destinationvalue) => source.Where("and", sourcevalue, sign, destinationvalue);

    public static ConditionClause WhereOr(this SelectQuery source, string sourcevalue, string sign, string destinationvalue) => source.Where("or", sourcevalue, sign, destinationvalue);

    private static ConditionClause Where(this SelectQuery source, string operatorToekn, string sourcevalue, string sign, string destinationvalue)
    {
        var c = new ConditionClause();
        c.Operator = operatorToekn;
        c.Source = new ValueClause() { Value = sourcevalue };
        c.Sign = sign;
        c.Destination = new ValueClause() { Value = destinationvalue };
        
        source.WhereClause.ConditionClause.Conditions.Add(c);
        return c;
    }

    public static void WhereAnd(this SelectQuery source, Action<ConditionGroupClause> action) => source.Where("and", action);

    public static void WhereOr(this SelectQuery source, Action<ConditionGroupClause> action) => source.Where("or", action);

    private static void Where(this SelectQuery source, string logicalOperator, Action<ConditionGroupClause> action)
    {
        var g = new ConditionGroupClause();
        g.Operator = logicalOperator;
        source.WhereClause.ConditionClause.GroupConditions.Add(g);
        action(g);
    }
}
