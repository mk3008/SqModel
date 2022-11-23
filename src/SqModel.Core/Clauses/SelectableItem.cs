using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class SelectableItem : IQueryable
{
    public SelectableItem(IValue query, string alias)
    {
        Query = query;
        Alias = alias;
    }

    public IValue Query { get; init; }

    public Dictionary<string, object?>? Parameters { get; set; } = null;

    public string Alias { get; init; }

    public string GetCommandText()
    {
        var query = Query.GetCommandText();
        if (Alias == Query.GetName()) return query;
        return $"{query} as {Alias}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        if (Parameters == null) return Query.GetParameters();
        return Parameters.Merge(Query.GetParameters());
    }
}