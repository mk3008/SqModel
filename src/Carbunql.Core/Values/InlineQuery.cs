﻿namespace Carbunql.Core.Values;

public class InlineQuery : QueryContainer
{
    public InlineQuery(IQueryCommandable query) : base(query)
    {
    }

    public override IEnumerable<Token> GetCurrentTokens(Token? parent)
    {
        var bracket = Token.BracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.BracketEnd(this, parent);
    }
}