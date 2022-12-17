namespace SqModel.Core.Clauses;

public abstract class TableBase : IQueryCommand
{
    public abstract IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens();

    public virtual string GetDefaultName() => string.Empty;
}