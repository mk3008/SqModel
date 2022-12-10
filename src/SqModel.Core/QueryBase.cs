using Cysharp.Text;
using SqModel.Core.Extensions;

namespace SqModel.Core;

public abstract class QueryBase : IQueryCommandable
{
    public abstract string GetCurrentCommandText();

    public abstract IDictionary<string, object?> GetCurrentParameters();

    public virtual string GetCommandText()
    {
        var sb = ZString.CreateStringBuilder();
        sb.Append(GetCurrentCommandText());
        if (OperatableQuery != null) sb.Append("\r\n" + OperatableQuery.GetCommandText());
        return sb.ToString();
    }

    public virtual IDictionary<string, object?> GetParameters()
    {
        var prm = GetCurrentParameters();
        prm = prm.Merge(OperatableQuery!.GetParameters());
        return prm;
    }

    public OperatableQuery? OperatableQuery { get; private set; }

    public QueryBase AddOperatableValue(string @operator, QueryBase query)
    {
        if (OperatableQuery != null) throw new InvalidOperationException();
        OperatableQuery = new OperatableQuery(@operator, query);
        return query;
    }

    public QueryCommand ToCommand()
    {
        return new QueryCommand(GetCommandText(), GetParameters());
    }
}