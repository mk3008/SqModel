namespace Carbunql.Core;

public interface IQueryCommand
{
    IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens();
}