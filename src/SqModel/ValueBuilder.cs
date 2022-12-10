namespace SqModel;

internal static class ValueBuilder
{
    public static IValueClause Create(TableClause table, string column)
        => new ColumnValue() { Table = table.AliasName, Column = column };

    public static IValueClause Create(string table, string column)
        => new ColumnValue() { Table = table, Column = column };

    public static IValueClause Create(bool value)
    {
        if (value) return new CommandValue() { CommandText = "true" };
        return new CommandValue() { CommandText = "false" };
    }

    public static IValueClause Create(object commandtext)
    {
        var val = commandtext?.ToString();
        if (val == null) throw new InvalidProgramException();
        var c = new CommandValue() { CommandText = val };
        return c;
    }

    public static IValueClause GetNullValue()
        => new CommandValue() { CommandText = "null" };

    public static IValueClause GetNotNullValue()
        => new CommandValue() { CommandText = "not null" };
}