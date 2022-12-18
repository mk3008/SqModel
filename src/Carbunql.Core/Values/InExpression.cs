namespace Carbunql.Core.Values;

public class InExpression : QueryContainer
{
    public InExpression(IQueryCommandable query) : base(query)
    {
    }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        yield return Token.Reserved(this, parent, "in");

        var bracket = Token.BracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.BracketEnd(this, parent);
    }
}