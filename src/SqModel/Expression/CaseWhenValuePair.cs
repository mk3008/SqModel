namespace SqModel.Expression;

public class CaseWhenValuePair : IThenCommand
{
    public ICondition? WhenExpression { get; set; } = null;

    public IValueClause? ThenValue { get; set; } = null;

    internal static string PrefixToken { get; set; } = "when";

    internal static string SufixToken { get; set; } = "then";

    internal static string OmitToken { get; set; } = "else";

    public Query ToQuery()
    {
        if (ThenValue == null) throw new InvalidProgramException();

        Query? q = null;

        // when condition then
        if (WhenExpression != null) q = WhenExpression.ToQuery().Decorate(PrefixToken, SufixToken);
        // else 
        else q = new Query() { CommandText = OmitToken };

        // ... value
        q = q.Merge(ThenValue.ToQuery());
        return q;
    }
}

public static class CaseWhenValuePairExtension
{
    public static CaseWhenValuePair When(this CaseWhenValuePair source, Action<Condition> action)
    {
        var c = new Condition();
        source.WhenExpression = c;
        action(c);
        return source;
    }

    public static CaseWhenValuePair WhenGroup(this CaseWhenValuePair source, Action<ConditionGroup> action)
    {
        var c = new ConditionGroup();
        source.WhenExpression = c;
        action(c);
        return source;
    }

    public static CaseWhenValuePair When(this CaseWhenValuePair source, LogicalExpression expression)
    {
        var c = new Condition() { Expression = expression };
        source.WhenExpression = c;
        return source;
    }

    public static CaseWhenValuePair When(this CaseWhenValuePair source, ICondition condition)
    {
        source.WhenExpression = condition;
        return source;
    }
}
