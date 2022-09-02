using SqModel;
using SqModel.Expression;
using SqModel.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqModel;

public class SelectItem : IValueContainer, IQueryable
{
    public IValueClause? Command { get; set; } = null;

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
        var c = new SelectQueryValue() { Query = new() { IsOneLineFormat = true } };
        source.Command = c;
        action(c.Query);
        return source;
    }
}