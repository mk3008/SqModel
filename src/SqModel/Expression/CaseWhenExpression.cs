namespace SqModel.Expression;

public class CaseWhenExpression : IValueClause
{
    public List<CaseWhenValuePair> Collection { get; set; } = new();

    internal static string PrefixToken { get; set; } = "case";

    internal static string SufixToken { get; set; } = "end";

    public string Conjunction { get; set; } = string.Empty;

    public Query ToQuery()
    {
        var q = new Query();
        Collection.ForEach(x => q = q.Merge(x.ToQuery()));
        q = q.Decorate(PrefixToken, SufixToken).InsertToken(Conjunction);
        return q;
    }

    public void AddParameter(string name, object? value)
        => throw new NotSupportedException();

    public string GetName() => string.Empty;
}

public static class CaseWhenExpressionExtension
{
    public static CaseWhenValuePair Add(this CaseWhenExpression source)
    {
        var c = new CaseWhenValuePair();
        source.Collection.Add(c);
        return c;
    }
}