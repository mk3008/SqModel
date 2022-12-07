using SqModel.Core.Extensions;

namespace SqModel.Core.Clauses;

public class SelectableItem : IQueryCommand, IQueryParameter, ISelectable
{
    public SelectableItem(ValueBase value, string alias)
    {
        Value = value;
        Alias = alias;
    }

    public ValueBase Value { get; init; }

    public string Alias { get; init; }

    public string GetCommandText()
    {
        var query = Value.GetCommandText();
        if (string.IsNullOrEmpty(Alias) || Alias == Value.GetDefaultName()) return query;
        return $"{query} as {Alias}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        throw new NotImplementedException();
    }
}