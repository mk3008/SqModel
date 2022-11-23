using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class SelectableTable : IQueryable, ITableAlias
{
    public SelectableTable(ITable query, string alias)
    {
        Query = query;
        Alias = alias;
    }

    internal ITable Query { get; init; }

    internal Dictionary<string, object?>? Parameters { get; set; }

    public string Alias { get; init; }

    public List<string>? ColumnAliases { get; set; }

    public string GetCommandText()
    {
        /*
         * query as alias(col1, col2, col3) 
         */
        var query = Query.GetCommandText();

        var alias = this.GetAliasCommand();
        if (!string.IsNullOrEmpty(alias)) return query;
        return $"{query} as {alias}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (Parameters == null) return Query.GetParameters();
        return Parameters.Merge(Parameters);
    }
}