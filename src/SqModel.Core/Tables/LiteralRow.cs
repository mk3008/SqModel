using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Tables;

public class LiteralRow : IQueryable
{
    public List<LiteralValue> Values { get; set; } = new();

    public string GetCommandText()
    {
        /*
         * (val1, val2, val3)
         */
        if (!Values.Any()) throw new IndexOutOfRangeException(nameof(Values));
        return Values.Select(x => x.GetCommandText()).ToString(", ", "(", ")");
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (!Values.Any()) throw new IndexOutOfRangeException(nameof(Values));
        return Values.Select(x => x.GetParameters()).Merge();
    }
}