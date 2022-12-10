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

    public string GetCommandText()
    {
        return Operator + "\r\n" + Query.GetCommandText();
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Query.GetParameters();
    }

    public QueryCommand ToCommand()
    {
        return Query.ToCommand();
    }
}