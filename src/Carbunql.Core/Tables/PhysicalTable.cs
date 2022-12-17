using Carbunql.Core.Clauses;

namespace Carbunql.Core.Tables;

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

    public override IEnumerable<(Type sender, string text, BlockType block, bool isReserved)> GetTokens()
    {
        var tp = GetType();
        if (!string.IsNullOrEmpty(Schame))
        {
            yield return (tp, Schame, BlockType.Default, false);
            yield return (tp, ".", BlockType.Default, true);
        }
        yield return (tp, Table, BlockType.Default, false);
    }

    public override string GetDefaultName() => Table;
}