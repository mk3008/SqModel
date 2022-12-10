namespace SqModel.Core.Values;

public class InExpression : QueryContainer
{
    public InExpression(IQueryCommandable query) : base(query)
    {
    }

    public override string GetCurrentCommandText() => "in (" + Query.GetCommandText() + ")";
}