using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class LiteralValue : ValueBase
{
    public LiteralValue(string commandText)
    {
        CommandText = commandText;
    }

    public string CommandText { get; init; }

    public override string GetCurrentCommandText() => CommandText;

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        return EmptyParameters.Get();
    }
}