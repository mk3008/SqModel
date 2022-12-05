using SqModel.Core.Clauses;

namespace SqModel.Core.Tables;

public class PhysicalTable : TableBase
{
    public PhysicalTable(string table)
    {
        Table = table;
    }

    public PhysicalTable(string schema, string table)
    {
        Schame = schema;
        Table = table;
    }

    public string? Schame { get; init; }

    public string Table { get; init; }

    public override string GetCommandText()
    {
        if (string.IsNullOrEmpty(Schame)) return Table;
        return $"{Schame}.{Table}";
    }

    public override string GetDefaultName() => Table;
}