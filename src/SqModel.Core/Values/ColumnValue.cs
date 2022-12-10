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

    public override string GetCurrentCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        if (string.IsNullOrEmpty(TableAlias))
        {
            sb.Append(Column);
        }
        else
        {
            sb.Append(TableAlias + "." + Column);
        }
        return sb.ToString();
    }

    public override string GetDefaultName()
    {
        if (OperatableValue == null) return Column;
        return string.Empty;
    }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        return EmptyParameters.Get();
    }
}