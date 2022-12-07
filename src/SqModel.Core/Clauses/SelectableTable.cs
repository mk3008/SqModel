using SqModel.Core.Extensions;
using SqModel.Core.Values;

namespace SqModel.Core.Clauses;

public class SelectableTable : IQueryCommand, IQueryParameter, ISelectable
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

    private string GetAliasCommand()
    {
        /*
         * alias(col1, col2, col3)
         */
        if (string.IsNullOrEmpty(Alias)) return string.Empty;
        if (ColumnAliases == null || !ColumnAliases.Any())
        {
            if (Alias == Table.GetDefaultName()) return string.Empty;
            return Alias;
        }

        return Alias + "(" + ColumnAliases.GetCommandText() + ")";
    }

    public string GetCommandText()
    {
        /*
         * query as alias(col1, col2, col3) 
         */

        var query = Table.GetCommandText();
        var alias = GetAliasCommand();
        if (string.IsNullOrEmpty(alias)) return query;
        return $"{query} as {alias}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm = Table.GetParameters();
        prm = prm.Merge(ColumnAliases!.GetParameters());
        return prm;
    }
}