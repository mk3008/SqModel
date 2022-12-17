namespace SqModel.Core.Values;

public class ExistsExpression : QueryContainer
{
    public ExistsExpression(IQueryCommandable query) : base(query)
    {
    }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, "exists", BlockType.Default, true);
        yield return (tp, "(", BlockType.Start, true);
        foreach (var item in Query.GetTokens()) yield return item;
        yield return (tp, ")", BlockType.End, true);
    }
}
