using Cysharp.Text;
using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core;

public abstract class QueryBase : IQueryCommandable
{
    public WithClause? WithClause { get; set; }

    public OrderClause? OrderClause { get; set; }

    public OperatableQuery? OperatableQuery { get; private set; }

    public LimitClause? LimitClause { get; set; }

    public QueryBase AddOperatableValue(string @operator, QueryBase query)
    {
        if (OperatableQuery != null) throw new InvalidOperationException();
        OperatableQuery = new OperatableQuery(@operator, query);
        return query;
    }

    public Dictionary<string, object?>? Parameters { get; set; }
    public abstract string GetCurrentCommandText();

    public abstract IDictionary<string, object?> GetCurrentParameters();

    public virtual string GetCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        if (WithClause != null) sb.Append(WithClause.GetCommandText() + "\r\n");
        sb.Append(GetCurrentCommandText());
        if (OrderClause != null) sb.Append("\r\n" + OrderClause.GetCommandText());
        if (OperatableQuery != null) sb.Append("\r\n" + OperatableQuery.GetCommandText());

        if (LimitClause != null) sb.Append("\r\n" + LimitClause.GetCommandText());

        return sb.ToString();
    }

    public virtual IDictionary<string, object?> GetParameters()
    {
        var prm = GetCurrentParameters();
        prm = prm.Merge(Parameters);
        prm = prm.Merge(WithClause!.GetParameters());
        prm = prm.Merge(OrderClause!.GetParameters());
        prm = prm.Merge(LimitClause!.GetParameters());
        prm = prm.Merge(OperatableQuery!.GetParameters());
        return prm;
    }

    public QueryCommand ToCommand()
    {
        return new QueryCommand(GetCommandText(), GetParameters());
    }
}