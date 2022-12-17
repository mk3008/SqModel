using System.Xml.Linq;

namespace SqModel.Core.Values;

public class InExpression : QueryContainer
{
    public InExpression(IQueryCommandable query) : base(query)
    {
    }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, "in", BlockType.Default, true);
        yield return (tp, "(", BlockType.Start, true);
        foreach (var item in Query.GetTokens()) yield return item;
        yield return (tp, ")", BlockType.End, true);
    }
}