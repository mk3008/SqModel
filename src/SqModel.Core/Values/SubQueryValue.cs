using SqModel.Core.Clauses;
using SqModel.Core.Extensions;

namespace SqModel.Core.Values;

public class SubQueryValue : IValue
{
    public SubQueryValue(IQueryable query)
    {
        Query = query;
    }

    public List<string>? Operators { get; set; }

    public IQueryable Query { get; init; }

    public string CommandText => Query.GetCommandText();

    public Dictionary<string, object?>? Parameters { get; set; }

    public string? GetName() => null;

    //public SubQueryValue? NestedValue { get; set; }

    public NestedValue? Nest { get; set; }

    public string GetCommandText()
    {
        if (NestedValue == null) return CommandText;
        return $"{CommandText} {NestedValue.GetCommandTextWithOperator()}";
    }

    public string GetCommandTextWithOperator()
    {
        if (Operators == null || !Operators.Any()) throw new NullReferenceException("Operators property is null.");
        return $"{Operators.ToString(" ")} {GetCommandText()}";
    }

    public IDictionary<string, object?> GetParameters()
    {
        var prm1 = Parameters != null ? Parameters : EmptyParameters.Get();
        var prm2 = Query.GetParameters();

        var prm = (new[] { prm1, prm2 }).Merge();
        if (NestedValue == null) return prm;
        return prm.Merge(NestedValue.GetParameters());
    }
}