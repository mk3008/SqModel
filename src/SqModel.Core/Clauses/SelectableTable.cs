using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class SelectableTable : IQueryCommand, ISelectable
{
    public SelectableTable(TableBase table, string alias)
    {
        Table = table;
        Alias = alias;
    }

    public SelectableTable(TableBase table, string alias, ValueCollection columnAliases)
    {
        Table = table;
        Alias = alias;
        ColumnAliases = columnAliases;
    }

    public TableBase Table { get; init; }

    public string Alias { get; init; }

    public ValueCollection? ColumnAliases { get; init; }

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetAliasTokens()
    {
        if (!string.IsNullOrEmpty(Alias) && Alias != Table.GetDefaultName())
        {
            var tp = GetType();
            yield return (tp, Alias, BlockType.Default, false);

            if (ColumnAliases != null)
            {
                yield return (tp, "(", BlockType.Default, true);
                foreach (var item in ColumnAliases.GetTokens()) yield return item;
                yield return (tp, ")", BlockType.Default, true);
            }
        }
    }

    public virtual IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        foreach (var item in Table.GetTokens()) yield return item;
        if (!string.IsNullOrEmpty(Alias) && Alias != Table.GetDefaultName())
        {
            var tp = GetType();
            yield return (tp, "as", BlockType.Default, true);
            foreach (var item in GetAliasTokens()) yield return item;
        }
    }
}