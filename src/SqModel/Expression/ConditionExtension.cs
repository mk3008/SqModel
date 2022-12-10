namespace SqModel.Expression;

public static class ConditionExtension
{

    public static LogicalExpression CaseWhen(this Condition source, Action<CaseWhenExpression> action)
    {
        var c = new CaseWhenExpression();
        action(c);
        return source.SetLeftValue(c);
    }

    public static LogicalExpression Case(this Condition source, TableClause table, string column, Action<CaseExpression> action)
    => source.Case(ValueBuilder.Create(table, column), action);

    public static LogicalExpression Case(this Condition source, string table, string column, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(table, column), action);

    public static LogicalExpression Case(this Condition source, string commandtext, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(commandtext), action);

    public static LogicalExpression Case(this Condition source, IValueClause value, Action<CaseExpression> action)
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
