using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class SubQueryValue : ValueBase
{
    public SubQueryValue(IQueryable query)
    {
        Query = query;
    }

    public IQueryable Query { get; init; }

    public override string GetCurrentCommandText() => $"({Query.GetCommandText()})";

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        return Query.GetParameters();
    }
}