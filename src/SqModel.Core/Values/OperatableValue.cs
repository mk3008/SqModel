using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class OperatableValue : IQueryCommand
{
    public OperatableValue(string @operator, ValueBase value)
    {
        Operator = @operator;
        Value = value;
    }

    public string Operator { get; init; }

    public ValueBase Value { get; init; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        if (!string.IsNullOrEmpty(Operator))
        {
            yield return (tp, Operator, BlockType.Default, true);
        }
        foreach (var item in Value.GetTokens()) yield return item;
    }
}