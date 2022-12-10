using SqModel;
using SqModel.Extension;

namespace SqModel;

public class SelectItem : IValueContainer, IQueryable
{
    public IValueClause? Command { get; set; } = null;

    public string ColumnName { get; set; } = string.Empty;

    /// <summary>
    /// Column alias name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    public Query ToQuery()
    {
        if (Command == null) throw new InvalidProgramException("Command property is null.");
        var q = Command.ToQuery();

        if (Name.IsNotEmpty() && Name != Command.GetName()) q.AddToken($"as {Name}");

        return q;
    }

    public string ToCommandText()
    {
        if (Command == null) throw new InvalidProgramException("Command property is null.");
        var q = Command.ToQuery();
        return q.CommandText;
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