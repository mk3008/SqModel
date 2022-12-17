using Carbunql.Core.Extensions;
using Carbunql.Core.Values;

namespace Carbunql.Core.Clauses;

public class CommonTable : SelectableTable
{
    public CommonTable(TableBase table, string alias) : base(table, alias)
    {
    }

    public CommonTable(TableBase table, string alias, ValueCollection columnAliases) : base(table, alias, columnAliases)
    {
    }

    public MaterializedType Materialized { get; set; } = MaterializedType.Undefined;

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();

        foreach (var item in GetAliasTokens()) yield return item;
        yield return (tp, "as", BlockType.Default, true);

        if (Materialized != MaterializedType.Undefined)
        {
            yield return (tp, Materialized.ToCommandText(), BlockType.Default, true);
        }

        foreach (var item in Table.GetTokens()) yield return item;
    }
}