using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class NegativeValue : ValueBase
{
    public NegativeValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, "not", BlockType.Default, true);
        foreach (var item in Inner.GetTokens()) yield return item;
    }
}