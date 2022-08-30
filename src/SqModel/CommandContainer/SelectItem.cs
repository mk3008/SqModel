using SqModel.Building;
using SqModel.Clause;
using SqModel.Command;
using SqModel.CommandContainer;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel.CommandContainer;


public class SelectItem : ICommandContainer, IQueryable
{
    public ICommand? Command { get; set; } = null;

    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Column alias name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public virtual Query ToQuery()
    {
        if (Command == null) throw new InvalidProgramException();
        var q = Command.ToQuery();
        if (Name.IsNotEmpty()) q.AddToken($"as {Name}");

        return q;
    }
}

public static class SelectItemExtension
{
    public static void All(this SelectItem source)
        => source.Value("*");

    public static void All(this SelectItem source, TableClause table)
        => source.Column(table, "*");

    public static void All(this SelectItem source, string table)
        => source.Column(table, "*");

    public static SelectItem InlineQuery(this SelectItem source, Action<SelectQuery> action)
    {
        var c = new SelectQueryCommand() { Query = new() };
        source.Command = c;
        action(c.Query);
        return source;
    }

    public static SelectItem Case(this SelectItem source, TableClause table, string column, Action<CaseExpression> action)
        => source.Case(CommandBuilder.Create(table, column), action);

    public static SelectItem Case(this SelectItem source, string table, string column, Action<CaseExpression> action)
        => source.Case(CommandBuilder.Create(table, column), action);

    public static SelectItem Case(this SelectItem source, string commandtext, Action<CaseExpression> action)
        => source.Case(CommandBuilder.Create(commandtext), action);

    public static SelectItem Case(this SelectItem source, ICommand value, Action<CaseExpression> action)
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



    //public static SelectItem Parameter(this SelectItem source, string name, object value)
    //{
    //    source.Command.AddParameter(name, value);
    //    return source;
    //}
}
