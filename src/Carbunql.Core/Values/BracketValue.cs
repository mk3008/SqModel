using Carbunql.Core.Clauses;

namespace Carbunql.Core.Values;

public class BracketValue : ValueBase
{
    public BracketValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        if (Inner == null) yield break;

        var bracket = Token.BracketStart(this, parent);
        yield return bracket;
        foreach (var item in Inner.GetTokens(bracket)) yield return item;
        yield return Token.BracketEnd(this, parent);
    }
}