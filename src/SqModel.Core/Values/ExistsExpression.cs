namespace SqModel.Core.Values;

public class ExistsExpression : QueryContainer
{
    public ExistsExpression(IQueryCommandable query) : base(query)
    {
    }

    public override string GetCurrentCommandText() => "exists (" + Query.GetCommandText() + ")";
}
