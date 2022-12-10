using Cysharp.Text;
using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class CommonTable : SelectableTable
{
    public CommonTable(TableBase table, string alias) : base(table, alias)
    {
    }

    public CommonTable(TableBase table, string alias, ValueCollection columnAliases) : base(table, alias, columnAliases)
    {
    }

    public MaterializedType Materialized { get; set; } = MaterializedType.Undefined;

    public override string GetCommandText()
    {
        var query = Table.GetCommandText();
        var alias = GetAliasCommand();
        if (string.IsNullOrEmpty(alias)) throw new NotSupportedException("alias is empty.");

        var sb = ZString.CreateStringBuilder();
        sb.Append(alias + " as ");
        if (Materialized != MaterializedType.Undefined) sb.Append(Materialized.ToCommandText() + " ");
        sb.Append(Table.GetCommandText());
        return sb.ToString();
    }
}