namespace SqModel.Expression;

public class CaseValuePair : IThenCommand
{
    public IValueClause? WhenValue { get; set; } = null;

    public IValueClause? ThenValue { get; set; } = null;

    private static string PrefixToken { get; set; } = "when";

    private static string SufixToken { get; set; } = "then";

    private static string OmitToken { get; set; } = "else";

    public Query ToQuery()
    {
        if (ThenValue == null) throw new InvalidProgramException();

        Query? q = null;

        // when value then
        if (WhenValue != null) q = WhenValue.ToQuery().Decorate(PrefixToken, SufixToken);
        // else 
        else q = new Query() { CommandText = OmitToken };

        // ... value
        q = q.Merge(ThenValue.ToQuery());
        return q;
    }
}

public static class CaseValuePairExtension
{
    public static CaseValuePair When(this CaseValuePair source, TableClause table, string column)
    => source.When(ValueBuilder.Create(table, column));

    public static CaseValuePair When(this CaseValuePair source, string table, string column)
    => source.When(ValueBuilder.Create(table, column));

    public static CaseValuePair When(this CaseValuePair source, object commandtext)
    => source.When(ValueBuilder.Create(commandtext));

    public static CaseValuePair When(this CaseValuePair source, IValueClause value)
    {
        source.WhenValue = value;
        return source;
    }
}


