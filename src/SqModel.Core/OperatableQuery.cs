namespace SqModel.Core;

public class OperatableQuery : IQueryCommandable
{
    public OperatableQuery(string @operator, QueryBase query)
    {
        Operator = @operator;
        Query = query;
    }

    public string Operator { get; init; }

    public QueryBase Query { get; init; }

    public IDictionary<string, object?> GetParameters()
    {
        return Query.GetParameters();
    }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, Operator, BlockType.Split, true);
        foreach (var item in Query.GetTokens()) yield return item;
    }

    public QueryCommand ToCommand()
    {
        return Query.ToCommand();
    }
}