using Cysharp.Text;
using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public class ColumnValue : ValueBase
{
    public ColumnValue(string column)
    {
        Column = column;
    }

    public ColumnValue(string table, string column)
    {
        TableAlias = table;
        Column = column;
    }

    public string TableAlias { get; set; } = string.Empty;

    public string Column { get; init; }

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens()
    {
        var tp = GetType();
        if (!string.IsNullOrEmpty(TableAlias))
        {
            yield return (tp, TableAlias, BlockType.Default, false);
            yield return (tp, ".", BlockType.Default, true);
        }
        yield return (tp, Column, BlockType.Default, false);
    }

    public override string GetDefaultName()
    {
        if (OperatableValue == null) return Column;
        return string.Empty;
    }
}