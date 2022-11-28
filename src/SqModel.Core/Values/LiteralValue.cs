using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class LiteralValue : ValueBase
{
    public LiteralValue(string commandText)
    {
        CommandText = commandText;
    }

    public string CommandText { get; init; }

    internal override string GetCurrentCommandText() => CommandText;
}