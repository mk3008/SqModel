using Carbunql.Core.Clauses;
using Carbunql.Core.Extensions;

namespace Carbunql.Core;

public abstract class QueryBase : IQueryCommandable
{
    public WithClause? WithClause { get; set; }

    public OperatableQuery? OperatableQuery { get; private set; }

    public OrderClause? OrderClause { get; set; }

    public LimitClause? LimitClause { get; set; }

    public QueryBase AddOperatableValue(string @operator, QueryBase query)
    {
        if (OperatableQuery != null) throw new InvalidOperationException();
        OperatableQuery = new OperatableQuery(@operator, query);
        return query;
    }

    public IDictionary<string, object?>? Parameters { get; set; }

    public virtual IDictionary<string, object?> GetParameters()
    {
        var prm = EmptyParameters.Get();
        prm = prm.Merge(Parameters);
        prm = prm.Merge(OperatableQuery!.GetParameters());
        return prm;
    }

    public abstract IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetCurrentTokens();

    public IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        if (WithClause != null) foreach (var item in WithClause.GetTokens()) yield return item;
        foreach (var item in GetCurrentTokens()) yield return item;
        if (OperatableQuery != null) foreach (var item in OperatableQuery.GetTokens()) yield return item;
        if (OrderClause != null) foreach (var item in OrderClause.GetTokens()) yield return item;
        if (LimitClause != null) foreach (var item in LimitClause.GetTokens()) yield return item;
    }

    public QueryCommand ToCommand()
    {
        var sql = GetTokens().ToString(" ");
        return new QueryCommand(sql, GetParameters());
    }
}