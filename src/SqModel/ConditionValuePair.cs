using SqModel.Building;
using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionValuePair : IReturnValueSettable
{
    public ValueContainer? ConditionExpression { get; set; } = null;

    public ValueClause? ConditionValue { get; set; } = null;

    public ValueClause ReturnValue { get; set; } = new();

    private static string PrefixToken { get; set; } = "when";

    private static string SufixToken { get; set; } = "then";

    private static string OmitToken { get; set; } = "else";

    public Query ToQuery()
    {
        Query? q = null;

        // when condition then
        if (ConditionExpression != null) q = ConditionExpression.ToQuery().Decorate(PrefixToken, SufixToken);
        // when value then
        else if (ConditionValue != null) q = ConditionValue.ToQuery().Decorate(PrefixToken, SufixToken);
        // else 
        else q = new Query() { CommandText = OmitToken };

        // ... value
        q = q.Merge(ReturnValue.ToQuery());
        return q;
    }

    public ValueClause SetReturnValue(ValueClause value)
    {
        ReturnValue = value;
        return value;
    }
}