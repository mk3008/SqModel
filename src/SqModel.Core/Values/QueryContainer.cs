using SqModel.Core.Clauses;

namespace SqModel.Core.Values;

public abstract class QueryContainer : ValueBase
{
    public QueryContainer(IQueryCommandable query)
    {
        Query = query;
    }

    public IQueryCommandable Query { get; init; }
}