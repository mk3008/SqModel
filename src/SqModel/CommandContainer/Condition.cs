using SqModel.Building;
using SqModel.Clause;
using SqModel.Command;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;

public class Condition : IQueryable, ICondition
{
    public string Operator { get; set; } = "and";

    public string SubOperator { get; set; } = String.Empty;

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

    public static void Equal(this Condition source, TableClause left, TableClause right, string column)
    {
        source.SetLeftValue(CommandBuilder.Create(left, column)).Equal(right, column);
    }

    public static void Equal(this Condition source, string left, string right, string column)
    {
        source.SetLeftValue(CommandBuilder.Create(left, column)).Equal(right, column);
    }

    public static LogicalExpression Value(this Condition source, object commandtext)
        => source.SetLeftValue(CommandBuilder.Create(commandtext));

    public static LogicalExpression Column(this Condition source, TableClause table, string column)
        => source.SetLeftValue(CommandBuilder.Create(table, column));

    public static LogicalExpression Column(this Condition source, string table, string column)
        => source.SetLeftValue(CommandBuilder.Create(table, column));

    private static LogicalExpression SetLeftValue(this Condition source, ICommand value)
    {
        var c = new LogicalExpression() { Left = value };
        source.Expression = c;
        return c;
    }

    public static LogicalExpression CaseWhen(this Condition source, Action<CaseWhenExpression> action)
    {
        var c = new CaseWhenExpression();
        action(c);
        return source.SetLeftValue(c);
    }

    public static LogicalExpression Case(this Condition source, TableClause table, string column, Action<CaseExpression> action)
    => source.Case(CommandBuilder.Create(table, column), action);

    public static LogicalExpression Case(this Condition source, string table, string column, Action<CaseExpression> action)
        => source.Case(CommandBuilder.Create(table, column), action);

    public static LogicalExpression Case(this Condition source, string commandtext, Action<CaseExpression> action)
        => source.Case(CommandBuilder.Create(commandtext), action);

    public static LogicalExpression Case(this Condition source, ICommand value, Action<CaseExpression> action)
    {
        var c = new CaseExpression() { Value = value };
        action(c);
        return source.SetLeftValue(c);
    }

    public static void Exists(this Condition source, Action<SelectQuery> action)
    {
        var q = new SelectQuery();
        q.IsOneLineFormat = true;

        var c = new ExistsExpression() { Query = q };
        source.Expression = c;
        action(q);
    }

    public static void Exists(this Condition source, SelectQuery query)
    {
        var c = new ExistsExpression() { Query = query };
        source.Expression = c;
    }
}