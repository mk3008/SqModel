using SqModel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class ConditionValuePair : IReturnValueClauseSettable
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

    public ValueClause SetReturnValueClause(ValueClause value)
    {
        ReturnValue = value;
        return value;
    }
}

public class CaseConditionValuePair : ConditionValuePair
    , IWhenValueSettable<IReturnValueClauseSettable>
{
    public IReturnValueClauseSettable SetWhenValueClause(ValueClause value)
    {
        ConditionValue = value;
        return this;
    }
}

public class CaseWhenConditionValuePair : ConditionValuePair
    , IWhenValueSettable<ISignValueClauseSettable<IReturnValueClauseSettable>>
    , ISignValueClauseSettable<IReturnValueClauseSettable>
{
    public ISignValueClauseSettable<IReturnValueClauseSettable> SetWhenValueClause(ValueClause value)
    {
        ConditionExpression ??= new();
        ConditionExpression.Source = value;
        return this;
    }

    public IReturnValueClauseSettable SetSignValueClause(string sign, ValueClause value)
    {
        if (ConditionExpression == null) throw new InvalidProgramException();

        var c = new ValueConjunction() { Sign = sign };
        ConditionExpression.ValueConjunction = c;
        c.Destination = value;

        return this;
    }
}


public interface IWhenValueSettable<T>
{
    T SetWhenValueClause(ValueClause value);
}

public static class IWhenValueSettableExtension
{
    public static T When<T>(this IWhenValueSettable<T> source, TableClause table, string column)
    => source.SetWhenValueClause(ValueBuilder.ToValue(table, column));

    public static T When<T>(this IWhenValueSettable<T> source, string table, string column)
        => source.SetWhenValueClause(ValueBuilder.ToValue(table, column));

    public static T When<T>(this IWhenValueSettable<T> source, string value)
        => source.SetWhenValueClause(ValueBuilder.ToValue(value));

    public static T When<T>(this IWhenValueSettable<T> source, ValueClause value)
        => source.SetWhenValueClause(value);
}

