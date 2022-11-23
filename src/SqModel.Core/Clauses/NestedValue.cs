namespace SqModel.Core.Clauses;

public class NestedValue : IValue
{
    public NestedValue(IValue value)
    {
        Value = value;
    }

    public List<string> Operators { get; set; } = new();

    public IValue Value { get; init; }

    public string GetCommandText()
    {
        throw new NotImplementedException();
    }
}
