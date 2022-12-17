using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class LiteralValue : ValueBase
{
    public LiteralValue(string commandText)
    {
        CommandText = commandText;
    }

    public string CommandText { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, CommandText, BlockType.Default, false);
    }
}