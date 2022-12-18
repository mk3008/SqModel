using Carbunql.Core.Clauses;

namespace Carbunql.Core.Tables;

public class VirtualTable : TableBase
{
    public VirtualTable(IQueryCommandable query)
    {
        Query = query;
    }

    public IQueryCommandable Query { get; init; }

    public override IEnumerable<Token> GetTokens(Token? parent)
    {
        var bracket = Token.BracketStart(this, parent);
        yield return bracket;
        foreach (var item in Query.GetTokens(bracket)) yield return item;
        yield return Token.BracketEnd(this, parent);
    }
}