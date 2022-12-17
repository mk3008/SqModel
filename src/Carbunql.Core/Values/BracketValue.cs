using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class BracketValue : ValueBase
{
    public BracketValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        if (Inner == null) throw new NullReferenceException();
        yield return (tp, "(", BlockType.Start, true);
        foreach (var item in Inner.GetTokens()) yield return item;
        yield return (tp, ")", BlockType.End, true);
    }
}