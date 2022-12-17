using Cysharp.Text;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class ValuesClause : QueryBase
{
    public ValuesClause(List<ValueCollection> rows)
    {
        Rows = rows;
    }

    public List<ValueCollection> Rows { get; init; } = new();

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        yield return (tp, "values", BlockType.Start, true);

        var isFirst = true;
        foreach (var item in Rows)
        {
            if (isFirst)
            {
                isFirst = false;
            }
            else
            {
                yield return (tp, ",", BlockType.Split, true);
            }
            yield return (tp, "(", BlockType.Start, true);
            foreach (var token in item.GetTokens()) yield return token;
            yield return (tp, ")", BlockType.End, true);
        }
        yield return (tp, string.Empty, BlockType.End, true);
    }
}