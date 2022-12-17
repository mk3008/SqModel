using Cysharp.Text;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class WindowFunction : IQueryCommand
{
    public ValueCollection? PartitionBy { get; set; }

    public ValueCollection? OrderBy { get; set; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        yield return (tp, "over", BlockType.Default, true);
        yield return (tp, "(", BlockType.Start, true);
        if (PartitionBy != null)
        {
            yield return (tp, "partition by", BlockType.Default, true);
            foreach (var item in PartitionBy.GetTokens()) yield return item;
        }
        if (OrderBy != null)
        {
            yield return (tp, "order by", BlockType.Default, true);
            foreach (var item in OrderBy.GetTokens()) yield return item;
        }
        yield return (tp, ")", BlockType.End, true);
    }
}