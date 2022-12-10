namespace SqModel;

public static class SelectQueryWithExtension
{
    public static CommonTable With(this SelectQuery source, string name)
    {
        var c = new CommonTable() { Name = name };
        source.With.Collection.Add(c);
        return c;
    }

    public static CommonTable As(this CommonTable source, Action<SelectQuery> action)
    {
        action(source.Query);
        return source;
    }

    public static CommonTable As(this CommonTable source, SelectQuery query)
    {
        source.Query = query;
        return source;
    }
}
