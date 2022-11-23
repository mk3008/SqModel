using SqModel.Core.Clauses;

namespace SqModel.Core.Tables;

internal class PhysicalTable : ITable
{
    public PhysicalTable(string table)
    {
        Table = table;
    }

    public string? Schame { get; set; }

    public string Table { get; init; }

    public string GetCommandText()
    {
        if (string.IsNullOrEmpty(Schame)) return Table;
        return $"{Schame}.{Table}";
    }

    public IDictionary<string, object?> GetParameters() => EmptyParameters.Get();
}