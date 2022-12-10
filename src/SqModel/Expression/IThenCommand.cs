namespace SqModel.Expression;

public interface IThenCommand
{
    IValueClause? ThenValue { get; set; }
}

public static class IThenCommandExtension
{
    public static void Then(this IThenCommand source, TableClause table, string column)
        => source.Then(ValueBuilder.Create(table, column));

    public static void Then(this IThenCommand source, string table, string column)
        => source.Then(ValueBuilder.Create(table, column));

    public static void Then(this IThenCommand source, object commandtext)
        => source.Then(ValueBuilder.Create(commandtext));

    public static void Then(this IThenCommand source, IValueClause value)
        => source.ThenValue = value;

    public static void Then(this IThenCommand source, bool value)
        => source.Then(ValueBuilder.Create(value));

    public static void ThenNull(this IThenCommand source)
        => source.Then(ValueBuilder.GetNullValue());

    public static void Else(this IThenCommand source, TableClause table, string column)
        => source.Then(ValueBuilder.Create(table, column));

    public static void Else(this IThenCommand source, string table, string column)
        => source.Then(ValueBuilder.Create(table, column));

    public static void Else(this IThenCommand source, object commandtext)
        => source.Then(ValueBuilder.Create(commandtext));

    public static void Else(this IThenCommand source, IValueClause value)
        => source.ThenValue = value;

    public static void Else(this IThenCommand source, bool value)
        => source.Then(ValueBuilder.Create(value));

    public static void ElseNull(this IThenCommand source)
        => source.Then(ValueBuilder.GetNullValue());
}