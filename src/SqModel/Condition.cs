using SqModel;
using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class Condition : IQueryable, ICondition
{
    public string Operator { get; set; } = "and";

    public string SubOperator { get; set; } = string.Empty;

    public string LeftTable { get; set; } = string.Empty;

    public string RightTable { get; set; } = string.Empty;

    public ILogicalExpression? Expression { get; set; } = null;

    public Query ToQuery()
    {
        if (Expression == null) throw new InvalidProgramException();
        var q = Expression.ToQuery();
        if (SubOperator.IsNotEmpty()) q = q.InsertToken(SubOperator);
        return q;
    }
}

public static class ConditionExtension
{
    public static Condition And(this Condition source)
        => source.SetOperator("and");

    public static Condition Or(this Condition source)
        => source.SetOperator("or");

    internal static Condition SetOperator(this Condition source, string @operator)
    {
        source.Operator = @operator;
        return source;
    }

    internal static Condition SetOperator(this Condition source, string @operator, string suboperator)
    {
        source.Operator = @operator;
        source.SubOperator = suboperator;
        return source;
    }

    public static Condition Not(this Condition source)
        => source.SetSubOperator("not");

    private static Condition SetSubOperator(this Condition source, string suboperator)
    {
        source.SubOperator = suboperator;
        return source;
    }

    public static void Equal(this Condition source, string column)
    {
        source.SetLeftValue(ValueBuilder.Create(source.LeftTable, column)).Equal(source.RightTable, column);
    }

    public static void Equal(this Condition source, TableClause left, TableClause right, string column)
    {
        source.SetLeftValue(ValueBuilder.Create(left, column)).Equal(right, column);
    }

    public static void Equal(this Condition source, string left, string right, string column)
    {
        source.SetLeftValue(ValueBuilder.Create(left, column)).Equal(right, column);
    }

    public static LogicalExpression Value(this Condition source, object commandtext)
        => source.SetLeftValue(ValueBuilder.Create(commandtext));

    public static LogicalExpression Column(this Condition source, TableClause table, string column)
        => source.SetLeftValue(ValueBuilder.Create(table, column));

    public static LogicalExpression Column(this Condition source, string table, string column)
        => source.SetLeftValue(ValueBuilder.Create(table, column));

    internal static LogicalExpression SetLeftValue(this Condition source, IValueClause value)
    {
        var c = new LogicalExpression() { Left = value };
        source.Expression = c;
        return c;
    }
}