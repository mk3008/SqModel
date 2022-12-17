using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class LikeExpression : ValueBase
{
    public LikeExpression(ValueBase value, ValueBase argument)
    {
        Value = value;
        Argument = argument;
    }

    public ValueBase Value { get; init; }

    public ValueBase Argument { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        foreach (var item in Value.GetTokens()) yield return item;
        yield return (tp, "like", BlockType.Default, true);
        foreach (var item in Argument.GetTokens()) yield return item;
    }
}