using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class SelectableItem : IQueryable
{
    public SelectableItem(IValue query, string alias)
    {
        Value = query;
        Alias = alias;
    }

    public IValue Value { get; init; }

    public Dictionary<string, object?>? Parameters { get; set; } = null;

    public string Alias { get; init; }

    public string GetCommandText()
    {
        var query = Value.GetCommandText();
        if (string.IsNullOrEmpty(Alias) || Alias == Value.GetDefaultName()) return query;
        return $"{query} as {Alias}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        return Parameters ?? EmptyParameters.Get();
    }
}