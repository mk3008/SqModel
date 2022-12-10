namespace SqModel;

public interface IValueClause : IQueryable
{
    string Conjunction { get; set; }

    void AddParameter(string name, object? value);

    string GetName();
}

public static class IValueClauseExtension
{
    public static IValueClause Conjunction(this IValueClause source, string sign)
    {
        source.Conjunction = sign;
        return source;
    }

    public static IValueClause Parameter(this IValueClause source, string key, object value)
    {
        source.AddParameter(key, value);
        return source;
    }
}