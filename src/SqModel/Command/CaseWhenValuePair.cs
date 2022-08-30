using SqModel.Building;
using SqModel.CommandContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Command;


public class CaseWhenValuePair : IThenCommand
{
    public LogicalExpression? WhenExpression { get; set; } = null;

    public ICommand? ThenValue { get; set; } = null;

    internal static string PrefixToken { get; set; } = "when";

    internal static string SufixToken { get; set; } = "then";

    internal static string OmitToken { get; set; } = "else";

    public Query ToQuery()
    {
        if (ThenValue == null) throw new InvalidProgramException();

        Query? q = null;

        // when condition then
        if (WhenExpression != null) q = WhenExpression.ToQuery().Decorate(PrefixToken, SufixToken);
        // else 
        else q = new Query() { CommandText = OmitToken };

        // ... value
        q = q.Merge(ThenValue.ToQuery());
        return q;
    }
}

public static class CaseWhenValuePairExtension
{
    public static CaseWhenValuePair When(this CaseWhenValuePair source, Action<LogicalExpression> action)
    {
        source.WhenExpression = new();
        action(source.WhenExpression);
        return source;
    }

    public static CaseWhenValuePair When(this CaseWhenValuePair source, LogicalExpression expression)
    {
        source.WhenExpression = expression;
        return source;
    }
}
