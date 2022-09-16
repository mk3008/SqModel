using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Expression;

public static class IValueContainerExtension
{
    public static IValueContainer Case(this IValueContainer source, TableClause table, string column, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(table, column), action);

    public static IValueContainer Case(this IValueContainer source, string table, string column, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(table, column), action);

    public static IValueContainer Case(this IValueContainer source, string commandtext, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(commandtext), action);

    public static IValueContainer Case(this IValueContainer source, IValueClause value, Action<CaseExpression> action)
    {
        var c = new CaseExpression() { Value = value };
        source.Command = c;
        action(c);
        return source;
    }

    public static IValueContainer CaseWhen(this IValueContainer source, Action<CaseWhenExpression> action)
    {
        var c = new CaseWhenExpression();
        source.Command = c;
        action(c);
        return source;
    }

    public static IValueContainer Strings(this IValueContainer source, Action<StringsExpression> action)
    {
        var c = new StringsExpression();
        source.Command = c;
        action(c);
        return source;
    }

    public static IValueContainer Concat(this IValueContainer source, Action<ConcatExpression> action)
    {
        var c = new ConcatExpression();
        source.Command = c;
        action(c);
        return source;
    }

    public static IValueContainer Concat(this IValueContainer source, string func, string split, Action<ConcatExpression> action)
    {
        var c = new ConcatExpression();
        source.Command = c;
        c.FunctionToken = func;
        c.SplitToken = split;
        action(c);
        return source;
    }
}