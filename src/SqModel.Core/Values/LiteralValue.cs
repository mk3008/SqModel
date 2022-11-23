using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class LiteralValue : IValue
{
    public LiteralValue(string commandText)
    {
        CommandText = commandText;
    }

    public string CommandText { get; init; }

    public Dictionary<string, object?>? Parameters { get; set; }

    public NestedValue? Nest { get; set; }

    public virtual string GetCommandText() => CommandText;

    public string? GetName() => null;

    public IDictionary<string, object?> GetParameters()
    {
        if (Parameters != null) return Parameters;
        return EmptyParameters.Get();
    }
}