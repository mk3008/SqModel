namespace SqModel.Expression;

public class StringsExpression : IValueClause
{
    public List<ValueContainer> Collection { get; } = new();

    public string Conjunction { get; set; } = String.Empty;

    public void AddParameter(string name, object? value)
        => throw new NotSupportedException();

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery(), " || "));
        return q;
    }

    public string GetName() => string.Empty;
}

public static class StringsExpressionExtension
{
    public static ValueContainer Add(this StringsExpression source)
    {
        var c = new ValueContainer();
        source.Collection.Add(c);
        return c;
    }
}
