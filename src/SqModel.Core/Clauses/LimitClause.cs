using Cysharp.Text;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class LimitClause : IQueryCommand
{
    public LimitClause(string text)
    {
        Conditions.Add(new LiteralValue(text));
    }

    public LimitClause(ValueBase item)
    {
        Conditions.Add(item);
    }

    public LimitClause(List<ValueBase> conditions)
    {
        conditions.ForEach(x => Conditions.Add(x));
    }

    public ValueCollection Conditions { get; init; } = new();

    public ValueBase? Offset { get; set; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, "limit", BlockType.Default, true);
        foreach (var item in Conditions.GetTokens()) yield return item;
        if (Offset != null)
        {
            yield return (tp, "offset", BlockType.Default, true);
            foreach (var item in Offset.GetTokens()) yield return item;
        }
    }
}