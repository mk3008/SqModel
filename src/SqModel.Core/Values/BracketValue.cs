using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class BracketValue : ValueBase
{
    public BracketValue(ValueBase inner)
    {
        Inner = inner;
    }

    public ValueBase Inner { get; init; }

    public override string GetCurrentCommandText()
    {
        if (Inner == null) throw new NullReferenceException();
        return "(" + Inner.GetCommandText() + ")";
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        return Inner.GetParameters();
    }
}