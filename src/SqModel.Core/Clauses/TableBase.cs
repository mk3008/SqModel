namespace SqModel.Core.Clauses;

public abstract class TableBase : IQueryCommand, IQueryParameter
{
    public abstract string GetCommandText();

    public virtual string GetDefaultName() => string.Empty;

    public virtual IDictionary<string, object?> GetParameters() => EmptyParameters.Get();
}