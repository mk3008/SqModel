namespace SqModel;

public class ColumnValue : IValueClause
{
    /// <summary>
    /// Table name.
    /// Specify an alias for the table clause.
    /// </summary>
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// Value name.
    /// </summary>
    public string Column { get; set; } = string.Empty;

    public string Conjunction { get; set; } = string.Empty;

    public void AddParameter(string name, object? value)
        => throw new NotSupportedException();

    public Query ToQuery()
        => new Query() { CommandText = $"{Table}.{Column}" }.InsertToken(Conjunction);

    public string GetName() => Column;
}