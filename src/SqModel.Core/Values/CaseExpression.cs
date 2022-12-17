using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class CaseExpression : ValueBase
{
    public CaseExpression()
    {
    }

    public CaseExpression(ValueBase condition)
    {
        CaseCondition = condition;
    }

    public ValueBase? CaseCondition { get; init; }

    public List<WhenExpression> WhenExpressions { get; init; } = new();

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, "case", BlockType.Start, true);
        if (CaseCondition != null) foreach (var item in CaseCondition.GetTokens()) yield return item;

        var isFirst = true;
        foreach (var item in WhenExpressions)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return (tp, string.Empty, BlockType.Split, true);
            }
            foreach (var token in item.GetTokens()) yield return token;
        }
        yield return (tp, "end", BlockType.End, true);
    }
}