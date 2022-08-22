using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class CaseExpression
{
    public ValueClause? Value { get; set; } = null;

    public List<ConditionValuePair> ConditionValues { get; set; } = new();

    private static string PrefixToken { get; set; } = "case";

    private static string SufixToken { get; set; } = "end";

    public Query ToQuery()
    {
        var q = Value?.ToQuery();
        q ??= new();
        ConditionValues.ForEach(x => q = q.Merge(x.ToQuery()));
        q.Decorate(PrefixToken, SufixToken);
        return q;
    }
}

public class CaseExpression<T> : CaseExpression where T : ConditionValuePair, new()
{
    internal T? CurrentConditionValuePair { get; set; } = null;

    public virtual T Add()
    {
        var c = new T();
        ConditionValues.Add(c);
        CurrentConditionValuePair = c;
        return c;
    }
}

