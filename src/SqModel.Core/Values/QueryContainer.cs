using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public abstract class QueryContainer : ValueBase
{
    public QueryContainer(IQueryable query)
    {
        Query = query;
    }

    public IQueryable Query { get; init; }

    public override IDictionary<string, object?> GetCurrentParameters()
    {
        return Query.GetParameters();
    }
}