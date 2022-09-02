using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.Expression;

public static class SelectItemExtension
{
    public static SelectItem Case(this SelectItem source, TableClause table, string column, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(table, column), action);

    public static SelectItem Case(this SelectItem source, string table, string column, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(table, column), action);

    public static SelectItem Case(this SelectItem source, string commandtext, Action<CaseExpression> action)
        => source.Case(ValueBuilder.Create(commandtext), action);

    public static SelectItem Case(this SelectItem source, IValueClause value, Action<CaseExpression> action)
    {
        var c = new CaseExpression() { Value = value };
        source.Command = c;
        action(c);
        return source;
    }

    public static SelectItem CaseWhen(this SelectItem source, Action<CaseWhenExpression> action)
    {
        var c = new CaseWhenExpression();
        source.Command = c;
        action(c);
        return source;
    }
}